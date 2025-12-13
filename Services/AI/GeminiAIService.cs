using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SalesOrderManagement.Services.AI
{
    public class GeminiAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiAIService> _logger;

        public GeminiAIService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAIService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AI:ApiKey"];
            _logger = logger;
        }

        public async Task<string> ExtractJsonAsync(string systemPrompt, string userContent)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("AI:ApiKey is missing. Returning mock data.");
                return GenerateMockJson();
            }

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
            Console.WriteLine($"DEBUG: Calling Gemini URL: {url.Replace(_apiKey, "HIDDEN_KEY")}");

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt + "\n\nData to process:\n" + userContent }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json"
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try 
            {
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                     var errorMsg = $"Gemini API Error: {response.StatusCode} - {responseString}";
                     Console.WriteLine(errorMsg);
                     
                     // Auto-debug: List available models
                     try 
                     {
                        var listUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";
                        var listResponse = await _httpClient.GetAsync(listUrl);
                        var listJson = await listResponse.Content.ReadAsStringAsync();
                        Console.WriteLine("--- AVAILABLE MODELS ---");
                        Console.WriteLine(listJson);
                        Console.WriteLine("------------------------");
                     }
                     catch { /* Ignore list error */ }

                     throw new HttpRequestException(errorMsg);
                }

                using var doc = JsonDocument.Parse(responseString);
                
                // Extract extraction from Gemini response structure
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                // Clean markdown code blocks if present
                var firstCurly = text.IndexOf('{');
                var firstSquare = text.IndexOf('[');
                int start = -1;
                int end = -1;

                // Determine if Object { or Array [ starts the JSON
                if (firstCurly != -1 && (firstSquare == -1 || firstCurly < firstSquare))
                {
                    start = firstCurly;
                    end = text.LastIndexOf('}');
                }
                else if (firstSquare != -1)
                {
                    start = firstSquare;
                    end = text.LastIndexOf(']');
                }

                if (start >= 0 && end > start)
                {
                    text = text.Substring(start, end - start + 1);
                }
                else
                {
                    // Fallback cleaning
                    text = text.Replace("```json", "").Replace("```", "").Trim();
                }
                
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                throw;
            }
        }

        private string GenerateMockJson()
        {
            return JsonSerializer.Serialize(new[]
            {
                new { ItemId = "MOCK-001", ItemName = "AI Extracted Item (Mock)", Quantity = 5, Price = 99.99, Brand = "MockBrand", Total = 499.95 }
            });
        }
    }
}
