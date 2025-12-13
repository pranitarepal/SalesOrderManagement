using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.Entities
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public int ItemId { get; set; }

        public int OrderId { get; set; }

        public string ProductName { get; set; }

        public string Manufacturer { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Subtotal { get; set; }

        [JsonIgnore]
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
