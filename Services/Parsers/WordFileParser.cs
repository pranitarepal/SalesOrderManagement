using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SalesOrderManagement.Models.DTOs;
using SalesOrderManagement.Services.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Services.Parsers
{
    public class WordFileParser : IFileParser
    {
        private readonly IAIService _aiService;

        public WordFileParser(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var sb = new StringBuilder();

            try
            {
                // OpenXml requires a seekable stream or MemoryStream. 
                // We copy to MemoryStream to ensure it works even if the input stream is not seekable.
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;

                using (var wordDoc = WordprocessingDocument.Open(ms, false))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    if (body != null)
                    {
                        // Extract text from all paragraphs
                        foreach (var paragraph in body.Descendants<Paragraph>())
                        {
                            sb.AppendLine(paragraph.InnerText);
                        }
                        
                        // Extract text from tables if necessary (InnerText usually covers it, but let's be sure)
                        // Actually paragraph.InnerText on Body covers paragraphs inside tables too in OpenXml usually, 
                        // but sometimes iteration needs to be recursive. 
                        // Body.InnerText gives all text but loses formatting entirely (newlines).
                        // Iterating paragraphs is a safe middle ground to keep some structure.
                    }
                }
            }
            catch (Exception ex)
            {
                return new PurchaseOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Failed to read Word file: {ex.Message}. Ensure it is a valid .docx file.",
                    DetectedType = "Word"
                };
            }

            var textContent = sb.ToString();

            if (string.IsNullOrWhiteSpace(textContent))
            {
                 return new PurchaseOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = "Word document contains no extractable text.",
                    DetectedType = "Word"
                };
            }

            // Reuse the same proven prompt from Excel and PDF parsers
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

                var result = JsonSerializer.Deserialize<PurchaseOrderResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });

                result = result ?? new PurchaseOrderResponseDto();
                result.DetectedType = "Word (AI Extraction)";
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
