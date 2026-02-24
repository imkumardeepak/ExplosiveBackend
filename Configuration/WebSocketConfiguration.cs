using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Configuration
{
    public static class WebSocketConfiguration
    {
        public static IApplicationBuilder ConfigureWebSockets(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var webSocketService = context.RequestServices.GetRequiredService<Services.WebSocketService>();

                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var token = context.Request.Query["token"].ToString();
                        if (string.IsNullOrEmpty(token))
                        {
                            context.Response.StatusCode = 401;
                            logger.LogWarning("WebSocket connection rejected: No token provided");
                            return;
                        }

                        var tokenHandler = new JwtSecurityTokenHandler();
                        try
                        {
                            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTSecret"));
                            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuer = true,
                                ValidIssuer = configuration.GetValue<string>("JWT:Issuer"),
                                ValidateAudience = true,
                                ValidAudience = configuration.GetValue<string>("JWT:Audience"),
                                ValidateLifetime = true
                            }, out var validatedToken);

                            context.User = principal;
                        }
                        catch (Exception ex)
                        {
                            context.Response.StatusCode = 401;
                            logger.LogError(ex, "WebSocket connection rejected: Invalid token");
                            return;
                        }

                        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Response.StatusCode = 400;
                            logger.LogWarning("WebSocket connection rejected: No user ID in token");
                            return;
                        }

                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await webSocketService.HandleWebSocketConnection(context, webSocket, userId);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        logger.LogWarning("Invalid WebSocket request");
                    }
                }
                else
                {
                    await next(context);
                }
            });

            return app;
        }

        public static void ConfigureNotificationEndpoint(this WebApplication app)
        {
            app.MapPost("/api/notifications", async (
                [FromBody] NotificationRequest request,
                HttpContext context,
                Services.WebSocketService webSocketService,
                ILogger<Program> logger) =>
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("Notification rejected: No user ID in token");
                    return Results.Unauthorized();
                }

                if (string.IsNullOrEmpty(request.Content))
                {
                    logger.LogWarning("Notification rejected: Empty content");
                    return Results.BadRequest("Notification content cannot be empty");
                }

                await webSocketService.SendNotificationAsync(userId, request.Content);
                return Results.Ok(new { Message = "Notification sent successfully" });
            });
        }
    }
}
