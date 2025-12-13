using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.Services.Parsers
{
    public class JsonFileParser : IFileParser
    {
        public async Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var items = await JsonSerializer.DeserializeAsync<List<PurchaseOrderItemDto>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PurchaseOrderItemDto>();

            return new PurchaseOrderResponseDto
            {
                Items = items,
                DetectedType = "JSON",
                Success = true
            };
        }
    }
}
