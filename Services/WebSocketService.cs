using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Peso_Baseed_Barcode_Printing_System_API.Services
{
	public class WebSocketService
	{
		private readonly ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();
		private readonly ISubscriber _redisSubscriber;
		private readonly ILogger<WebSocketService> _logger;
		private readonly string _redisChannel = "websocket-messages";
		private readonly JsonSerializerOptions _jsonOptions;

		public WebSocketService(IConnectionMultiplexer redis, ILogger<WebSocketService> logger)
		{
			_redisSubscriber = redis.GetSubscriber();
			_logger = logger;

			_jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			_redisSubscriber.Subscribe(_redisChannel, async (channel, message) =>
			{
				await HandleRedisMessage(message);
			});
			_logger.LogInformation("WebSocketService initialized and subscribed to Redis channel: {Channel}", _redisChannel);
		}

		public async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket, string userId)
		{
			_connections[userId] = webSocket;
			_logger.LogInformation("WebSocket connected for user {UserId}", userId);

			var buffer = new byte[1024 * 4];
			try
			{
				while (webSocket.State == WebSocketState.Open)
				{
					var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
					if (result.MessageType == WebSocketMessageType.Text)
					{
						var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
						_logger.LogInformation("Received WebSocket message from {UserId}: {Message}", userId, message);

						if (string.IsNullOrWhiteSpace(message))
						{
							_logger.LogWarning("Empty or null message received from {UserId}", userId);
							continue;
						}

						Message messageObj;
						try
						{
							var dto = JsonSerializer.Deserialize<MessageDto>(message, _jsonOptions);
							if (dto == null)
							{
								_logger.LogWarning("Deserialized message is null from {UserId}: {Message}", userId, message);
								continue;
							}
							messageObj = new Message
							{
								SenderId = dto.SenderId?.ToString(),
								ReceiverId = dto.ReceiverId?.ToString(),
								Content = dto.Content
							};
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize message from {UserId}: {Message}", userId, message);
							continue;
						}

						if (string.IsNullOrEmpty(messageObj.SenderId) || messageObj.SenderId != userId)
						{
							_logger.LogWarning("Invalid message from {UserId}: SenderId mismatch or empty, Message: {Message}", userId, message);
							continue;
						}

						await _redisSubscriber.PublishAsync(_redisChannel, JsonSerializer.Serialize(messageObj));
						_logger.LogInformation("Published message to Redis from {UserId}: {Message}", userId, message);
					}
					else if (result.MessageType == WebSocketMessageType.Close)
					{
						_logger.LogInformation("WebSocket closing for {UserId}", userId);
						break;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling WebSocket for {UserId}", userId);
			}
			finally
			{
				_connections.TryRemove(userId, out _);
				if (webSocket.State == WebSocketState.Open)
				{
					await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
				}
				_logger.LogInformation("WebSocket disconnected for {UserId}", userId);
			}
		}

		private async Task HandleRedisMessage(RedisValue message)
		{
			_logger.LogInformation("Received message on Redis channel: {Message}", message);

			if (message.IsNullOrEmpty)
			{
				_logger.LogWarning("Empty or null message received on Redis channel");
				return;
			}

			Message messageObj;
			try
			{
				var dto = JsonSerializer.Deserialize<MessageDto>(message, _jsonOptions);
				if (dto == null)
				{
					_logger.LogWarning("Deserialized Redis message is null: {Message}", message);
					return;
				}
				messageObj = new Message
				{
					SenderId = dto.SenderId?.ToString(),
					ReceiverId = dto.ReceiverId?.ToString(),
					Content = dto.Content
				};
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Failed to deserialize Redis message: {Message}", message);
				return;
			}

			if (messageObj.Content == "connection_established")
			{
				_logger.LogInformation("Skipping connection_established for user {SenderId}", messageObj.SenderId);
				return;
			}

			if (!string.IsNullOrEmpty(messageObj.ReceiverId))
			{
				if (_connections.TryGetValue(messageObj.ReceiverId, out var receiverSocket) && receiverSocket.State == WebSocketState.Open)
				{
					var response = $"From {messageObj.SenderId}: {messageObj.Content} To: {messageObj.ReceiverId}";
					var messageBytes = Encoding.UTF8.GetBytes(response);
					await receiverSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
					_logger.LogInformation("Sent message to {ReceiverId}: {Response}", messageObj.ReceiverId, response);
				}
				else
				{
					_logger.LogWarning("No open connection for receiver {ReceiverId}", messageObj.ReceiverId);
				}
			}
			else
			{
				var notification = $"Notification: {messageObj.Content}";
				var notificationBytes = Encoding.UTF8.GetBytes(notification);
				int sentCount = 0;
				foreach (var socket in _connections.Values)
				{
					if (socket.State == WebSocketState.Open)
					{
						await socket.SendAsync(new ArraySegment<byte>(notificationBytes), WebSocketMessageType.Text, true, CancellationToken.None);
						sentCount++;
					}
				}
				_logger.LogInformation("Broadcast notification to {SentCount} clients: {Notification}", sentCount, notification);
			}
		}

		public async Task SendNotificationAsync(string senderId, string content)
		{
			if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(content))
			{
				_logger.LogWarning("Invalid notification parameters: SenderId={SenderId}, Content={Content}", senderId, content);
				return;
			}

			var message = new Message
			{
				SenderId = senderId,
				ReceiverId = "",
				Content = content
			};

			await _redisSubscriber.PublishAsync(_redisChannel, JsonSerializer.Serialize(message));
			_logger.LogInformation("Published notification from {SenderId}: {Content}", senderId, content);
		}
	}


	public class MessageDto
	{
		[JsonPropertyName("senderId")]
		public object SenderId { get; set; } // Handle int or string
		[JsonPropertyName("receiverId")]
		public object ReceiverId { get; set; } // Handle int or string
		[JsonPropertyName("content")]
		public string Content { get; set; }
	}
}
