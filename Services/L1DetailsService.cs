using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Services
{
	public class L1DetailsService
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<L1DetailsService> _logger;

		public L1DetailsService(ApplicationDbContext context , ILogger<L1DetailsService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public string GetQuarter(string dateString)
		{
			if (string.IsNullOrWhiteSpace(dateString) || dateString.Length < 5)
				return "-1";

			string month = dateString.Substring(3, 2);

			return month switch
			{
				"04" or "05" or "06" => "1",
				"07" or "08" or "09" => "2",
				"10" or "11" or "12" => "3",
				"01" or "02" or "03" => "4",
				_ => "-1"
			};
		}

		public async Task<BatchDetails> GetOrCreateBatchAsync(string plantCode, DateTime mfgDate, decimal requiredWeight)
		{

			var availableBatch = await _context.BatchDetails
				.Where(b => b.PlantCode == plantCode &&
						   b.MfgDate == mfgDate &&
						   b.RemainingCapacity >= requiredWeight)
				.OrderByDescending(b => b.Id) 
				.FirstOrDefaultAsync();

			if (availableBatch != null)
			{
				_logger.LogInformation($"Using existing batch {availableBatch.BatchCode} with remaining capacity {availableBatch.RemainingCapacity}");
				return availableBatch;
			}
			return await CreateNewBatchAsync(plantCode, mfgDate);
		}

		public async Task<BatchDetails> CreateNewBatchAsync(string plantCode, DateTime mfgDate)
		{
			

			try
			{
				var batchConfig = await _context.BatchMasters
					.FirstOrDefaultAsync(b => b.PlantCode == plantCode);

				if (batchConfig == null)
				{
					return null;
				}

				var lastBatch = ShouldResetSequence(batchConfig, plantCode, mfgDate);
				

				string newBatchCode;
				if (lastBatch != null)
				{

					var numericPart = Regex.Match(lastBatch.BatchCode, @"\d+").Value;
					if (int.TryParse(numericPart, out int lastSequence))
					{
						var prefix = lastBatch.BatchCode.Substring(0, lastBatch.BatchCode.Length - numericPart.Length);
						newBatchCode = $"{prefix}{(lastSequence + 1).ToString().PadLeft(numericPart.Length, '0')}";
					}
					else
					{
						newBatchCode = GenerateStandardBatchCode(batchConfig);
					}
				}
				else
				{
					newBatchCode = GenerateStandardBatchCode(batchConfig);
				}


				var newBatch = new BatchDetails
				{
					PlantCode = plantCode,
					MfgDate = mfgDate,
					BatchCode = newBatchCode,
					TotalCapacity = batchConfig.BatchSize,
					RemainingCapacity = batchConfig.BatchSize 
				};

				await _context.BatchDetails.AddAsync(newBatch);
				await _context.SaveChangesAsync();


				_logger.LogInformation($"Created new batch {newBatchCode} for plant {plantCode}");

				return newBatch;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create new batch");
				throw;
			}
		}

		private string GenerateStandardBatchCode(BatchMasters config)
		{
			var (prefix, year,month ,sequence) = ParseBatchCode(config.BatchCode, config.BatchFormat);
			sequence++;
			return FormatBatchCode(config.BatchFormat, prefix, year, month, sequence);
		}

		public async Task<(string newBatchCode, BatchDetails newBatchDetail)> GenerateNextBatchCodeAsync(string plantCode, DateTime mfgDate)
		{
			
			try
			{
				// Get batch configuration
				var batchConfig = await _context.BatchMasters
					.FirstOrDefaultAsync(b => b.PlantCode == plantCode && b.BatchType=='A');

				if (batchConfig == null)
				{
					throw new ArgumentException($"No batch configuration found for plant code: {plantCode}");
				}

				// Parse the current batch code
				var (prefix, year, month, sequence) = ParseBatchCode(batchConfig.BatchCode, batchConfig.BatchFormat);

	

				sequence++;

				// Generate new batch code
				string newBatchCode = FormatBatchCode(batchConfig.BatchFormat, prefix, year, month, sequence);


				// Create new batch detail
				var newBatchDetail = new BatchDetails
				{
					PlantCode = plantCode,
					MfgDate = mfgDate,
					BatchCode = newBatchCode,
					TotalCapacity = batchConfig.BatchSize,
					RemainingCapacity = batchConfig.BatchSize
				};

				await _context.BatchDetails.AddAsync(newBatchDetail);
				await _context.SaveChangesAsync();

				return (newBatchCode, newBatchDetail);
			}
			catch
			{
				throw;
			}
		}

		private (string prefix, string year, string month, int sequence) ParseBatchCode(string batchCode, string format)
		{
			var formatParts = format.Split('-');
			int currentPos = 0;

			string prefix = "";
			string year = "";
			string month = "";
			int sequence = 0;

			foreach (var part in formatParts)
			{
				var match = Regex.Match(part, @"([^\(]+)\((\d+)\)");
				if (!match.Success) continue;

				string elementType = match.Groups[1].Value.ToLower();
				int length = int.Parse(match.Groups[2].Value);

				if (currentPos + length > batchCode.Length)
				{
					throw new ArgumentException($"Batch code '{batchCode}' doesn't match format '{format}'");
				}

				string value = batchCode.Substring(currentPos, length);
				currentPos += length;

				switch (elementType)
				{
					case "prefix":
						prefix = value;
						break;
					case "year":
						year = length==2 ? DateTime.Now.ToString("yy") : DateTime.Now.ToString("yyyy");
						break;
					case "month":
						month = length == 2 ? DateTime.Now.ToString("MM") : DateTime.Now.ToString("MMM");
						break;
					case "serial":
					case "seq":
						sequence = int.Parse(value);
						break;
					case "plantcode":
						prefix = value;
						break;
				}
			}

			return (prefix, year, month, sequence);
		}

		private string FormatBatchCode(string format, string prefix, string year, string month, int sequence)
		{
			var formatParts = format.Split('-');
			var sb = new StringBuilder();

			foreach (var part in formatParts)
			{
				var match = Regex.Match(part, @"([^\(]+)\((\d+)\)");
				if (!match.Success) continue;

				string elementType = match.Groups[1].Value.ToLower();
				int length = int.Parse(match.Groups[2].Value);

				switch (elementType)
				{
					case "prefix":
						sb.Append(prefix.PadLeft(length, '0'));
						break;
					case "year":
						// Use last N digits of year based on requested length
						string yearStr = year.ToString();
						sb.Append(yearStr.Substring(yearStr.Length - Math.Min(length, yearStr.Length)).PadLeft(length, '0'));
						break;
					case "month":
						sb.Append(month.ToString().PadLeft(length, '0'));
						break;
					case "serial":
					case "seq":
						sb.Append(sequence.ToString().PadLeft(length, '0'));
						break;
					case "plantcode":
						sb.Append(prefix.PadLeft(length, '0'));
						break;
				}
			}

			return sb.ToString();
		}

		private  BatchDetails ShouldResetSequence(BatchMasters batchMasters , string plantCode,DateTime mfgdt)
		{
			switch (batchMasters.ResetType)
			{
				case 'D': // Daily reset
					return  _context.BatchDetails
						.Where(b => b.PlantCode == plantCode && b.MfgDate.Date == mfgdt.Date)
						.OrderByDescending(b => b.Id)
						.FirstOrDefault();
				case 'M': // Monthly reset
					return _context.BatchDetails
						.Where(b => b.PlantCode == plantCode && b.MfgDate.Month == mfgdt.Month)
						.OrderByDescending(b => b.Id)
						.FirstOrDefault();

				case 'Y': // Yearly reset
					return _context.BatchDetails
						.Where(b => b.PlantCode == plantCode && b.MfgDate.Year == mfgdt.Year)
						.OrderByDescending(b => b.Id)
						.FirstOrDefault();
			}

			return _context.BatchDetails
						.Where(b => b.PlantCode == plantCode)
						.OrderByDescending(b => b.Id)
						.FirstOrDefault();
		}
	}
}
