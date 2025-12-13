using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using SalesOrderManagement.Services.AI;
using ClosedXML.Excel;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Services.Parsers
{
    public class ExcelFileParser : IFileParser
    {
        private readonly IAIService _aiService;

        public ExcelFileParser(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var response = new PurchaseOrderResponseDto();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed();

            if (!rows.Any()) return response;

            // Given the requirement that "format can be different, columns can be different", 
            // we will bypass the rigid "standard" check and always use the AI to identify and extract data.
            return await ParseWithAI(worksheet);

            // AI Fallback for unstructured/unknown formats
            return await ParseWithAI(worksheet);
        }

        private List<PurchaseOrderItemDto> ParseStandard(IEnumerable<IXLRangeRow> rows)
        {
            var items = new List<PurchaseOrderItemDto>();
            foreach (var row in rows)
            {
                try
                {
                    items.Add(new PurchaseOrderItemDto
                    {
                        ItemId = row.Cell(1).GetValue<string>(),
                        ItemName = row.Cell(2).GetValue<string>(),
                        Quantity = row.Cell(3).GetValue<int>(),
                        Price = row.Cell(4).GetValue<decimal>(),
                        Brand = row.Cell(5).GetValue<string>(),
                        Total = row.Cell(6).GetValue<decimal>()
                    });
                }
                catch { /* Ignore bad rows */ }
            }
            return items;
        }

        private async Task<PurchaseOrderResponseDto> ParseWithAI(IXLWorksheet worksheet)
        {
            // Convert worksheet to CSV string for context
            var sb = new StringBuilder();
            var range = worksheet.RangeUsed();
            var lastCol = range.LastColumn().ColumnNumber();

            foreach (var row in range.RowsUsed())
            {
                var cells = new List<string>();
                for (int i = 1; i <= lastCol; i++)
                {
                     cells.Add(row.Cell(i).GetValue<string>());
                }
                sb.AppendLine(string.Join(",", cells));
            }

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
                var json = await _aiService.ExtractJsonAsync(prompt, sb.ToString());
                
                // DEBUG LOGGING
                Console.WriteLine("--- RAW AI RESPONSE ---");
                Console.WriteLine(json);
                Console.WriteLine("---------------------");

                 // Handle potential markdown wrapping
                json = json.Replace("```json", "").Replace("```", "").Trim();
                
                // Ensure we start with { (Basic Check)
                if (!json.StartsWith("{"))
                {
                    // If AI returned array [...] usually means it ignored top level instruction.
                    // We can try to wrap it if it starts with [
                    if (json.StartsWith("["))
                    {
                        json = "{ \"orderLineItems\": " + json + " }";
                    }
                }
                
                var result = JsonSerializer.Deserialize<PurchaseOrderResponseDto>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString // Allow numbers as strings just in case
                });
                result = result ?? new PurchaseOrderResponseDto();
                result.DetectedType = "Excel (AI Extraction)";
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                // Rethrow to let the controller handle it and show error to user
                throw new Exception($"AI Extraction Failed: {ex.Message}", ex);
            }
        }
    }
}
