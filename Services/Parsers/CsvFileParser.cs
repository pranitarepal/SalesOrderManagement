using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using SalesOrderManagement.Services.AI;

namespace SalesOrderManagement.Services.Parsers
{
    public class CsvFileParser : IFileParser
    {
        private readonly IAIService _aiService;

        public CsvFileParser(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var textContent = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(textContent))
            {
                return new PurchaseOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = "File is empty.",
                    DetectedType = "Text/CSV"
                };
            }

            // Reuse the same proven prompt
             var prompt = @"
You are an AI specialized in data extraction.
Extract purchase order details from the provided text.

Return a SINGLE JSON OBJECT with this exact structure:
{
  ""vendorName"": ""string or null"",
  ""vendorAddress"": ""string or null"",
  ""purchaseOrderNumber"": ""string or null"",
  ""purchaseOrderDate"": ""YYYY-MM-DD or null"",
  ""currency"": ""USD/EUR/etc or null"",  
  ""orderLineItems"": [
    {
      ""itemName"": ""Item Name or ID"",
      ""quantity"": 0,
      ""manufatcurer"": ""string or null""
    }
  ]
}

- Map 'Description', 'Item Name', or 'Part Number' to 'itemName'.
- Map 'Brand' or 'Manufacturer' to 'manufatcurer'.
- Do NOT return unitPrice, lineTotal, or other fields unless requested.
- If the file contains only a list of items and no vendor header, set top-level fields to null and fill orderLineItems.
- Return ONLY the JSON object. No markdown.
";

            try
            {
                var json = await _aiService.ExtractJsonAsync(prompt, textContent);

                // Handle potential markdown wrapping
                json = json.Replace("```json", "").Replace("```", "").Trim();

                // Ensure we start with { (Basic Check)
                if (!json.StartsWith("{"))
                {
                   if (json.StartsWith("["))
                   {
                       json = "{ \"orderLineItems\": " + json + " }";
                   }
                }

                var result = System.Text.Json.JsonSerializer.Deserialize<PurchaseOrderResponseDto>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                });

                result = result ?? new PurchaseOrderResponseDto();
                result.DetectedType = "Text/CSV (AI Extraction)";
                result.Success = true;
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"AI Extraction Failed: {ex.Message}", ex);
            }
        }
    }
}
