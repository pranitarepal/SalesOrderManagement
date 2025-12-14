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

    public class InvalidItemDetailsDto
    {
        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }
        
        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }
        
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class PurchaseOrderApiResponseDto
    {
        [JsonPropertyName("orderLineItems")]
        public List<PurchaseOrderApiItemDto> OrderLineItems { get; set; } = new List<PurchaseOrderApiItemDto>();
        
        [JsonPropertyName("invalidItems")]
        public List<InvalidItemDetailsDto> InvalidItems { get; set; } = new List<InvalidItemDetailsDto>();
        
        public string SourceFileName { get; set; }
        public string DetectedType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
