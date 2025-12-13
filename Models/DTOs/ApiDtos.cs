using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.DTOs
{
    public class PurchaseOrderApiItemDto
    {
        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }
        
        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }
        
        [JsonPropertyName("manufatcurer")]
        public string Brand { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
    }

    public class PurchaseOrderApiResponseDto
    {
        [JsonPropertyName("orderLineItems")]
        public List<PurchaseOrderApiItemDto> OrderLineItems { get; set; } = new List<PurchaseOrderApiItemDto>();
        
        public string SourceFileName { get; set; }
        public string DetectedType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
