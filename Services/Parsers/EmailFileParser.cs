using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using SalesOrderManagement.Services.AI;
using MsgReader.Outlook;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Services.Parsers
{
    public class EmailFileParser : IFileParser
    {
        private readonly IAIService _aiService;

        public EmailFileParser(IAIService aiService)
        {
            _aiService = aiService;
        }

        
        public async Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var sb = new StringBuilder();
            
            // MsgReader expects a seekable stream. We load into memory first.
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;

            try
            {
                using (var msg = new Storage.Message(ms))
                {
                    var bodyText = msg.BodyText;
                    var bodyHtml = msg.BodyHtml;
                    var subject = msg.Subject;

                    sb.AppendLine($"Subject: {subject}");

                    if (!string.IsNullOrWhiteSpace(bodyText))
                    {
                        sb.AppendLine(bodyText);
                    }
                    else if (!string.IsNullOrWhiteSpace(bodyHtml))
                    {
                        sb.AppendLine(bodyHtml);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback: If it's an "Invalid header signature", it might be an EML (MIME) file.
                try 
                {
                    ms.Position = 0;
                    using var reader = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: -1, leaveOpen: true);
                    sb.Clear(); 
                    sb.AppendLine(await reader.ReadToEndAsync());
                }
                catch
                {
                    // If fallback also fails, return original error
                    return new PurchaseOrderResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"Failed to read .msg file: {ex.Message} (and fallback text read failed)",
                        DetectedType = "Email (.msg)"
                    };
                }
            }

            var textContent = sb.ToString();

            if (string.IsNullOrWhiteSpace(textContent))
            {
                 return new PurchaseOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = "Email contains no extractable text.",
                    DetectedType = "Email (.msg)"
                };
            }

            // Reuse the same proven prompt
             var prompt = @"
You are an AI specialized in data extraction.
Extract purchase order details from the provided email text.

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
                result.DetectedType = "Email (.msg) (AI Extraction)";
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
