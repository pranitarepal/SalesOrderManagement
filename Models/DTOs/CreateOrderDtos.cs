using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.DTOs
{
    public class CreatePurchaseOrderRequestDto
    {
        [JsonPropertyName("orderLineItems")]
        public List<CreateOrderLineItemDto> OrderLineItems { get; set; }

        [JsonPropertyName("customer")]
        public CustomerDto Customer { get; set; }
    }

    public class CreateOrderLineItemDto
    {
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }

        [JsonPropertyName("ProductName")]
        public string? ItemName { get; set; }

        [JsonPropertyName("Qty")]
        public int Quantity { get; set; }

        // Mispelling intentional to match user payload
        [JsonPropertyName("Manufacturer")]
        public string? Manufacturer { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
    }

    public class CustomerDto
    {
        [JsonPropertyName("CustomerId")]
        public string UserId { get; set; }
    }
}
