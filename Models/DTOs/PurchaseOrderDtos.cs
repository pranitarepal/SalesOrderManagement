using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.DTOs
{
    public class PurchaseOrderItemDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ItemId { get; set; }

        // Map "description" from AI prompt to "ItemName"
        [JsonPropertyName("itemName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ItemName { get; set; }
        
        [JsonPropertyName("quantity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Quantity { get; set; }
        
        [JsonPropertyName("unitPrice")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("manufatcurer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Brand { get; set; }
        
        [JsonPropertyName("lineTotal")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Total { get; set; }
    }

    public class PurchaseOrderResponseDto
    {
        [JsonPropertyName("orderLineItems")]
        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VendorName { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VendorAddress { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PurchaseOrderNumber { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? PurchaseOrderDate { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Currency { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Subtotal { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Tax { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? GrandTotal { get; set; }

        public string SourceFileName { get; set; }
        public string DetectedType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
