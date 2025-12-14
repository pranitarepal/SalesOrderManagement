using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.DTOs
{
    public class OrderDetailsDto
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
        
        [JsonPropertyName("orderStatus")]
        public string? OrderStatus { get; set; }
        
        [JsonPropertyName("orderDate")]
        public DateTime? OrderDate { get; set; }
        
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
        
        [JsonPropertyName("itemDetails")]
        public List<OrderItemDto> ItemDetails { get; set; } = new List<OrderItemDto>();
    }

    public class OrderListDto
    {
        [JsonPropertyName("orders")]
        public List<OrderDetailsDto> Orders { get; set; } = new List<OrderDetailsDto>();
    }

    public class OrderItemDto
    {
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }
        
        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }
        
        [JsonPropertyName("manufacturer")]
        public string? Manufacturer { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("qty")]
        public int? Qty { get; set; }
        
        [JsonPropertyName("subtotal")]
        public decimal? Subtotal { get; set; }
    }
}
