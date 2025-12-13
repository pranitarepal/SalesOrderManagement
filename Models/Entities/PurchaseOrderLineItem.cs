using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.Entities
{
    [Table("OrderLineItems")]
    public class PurchaseOrderLineItem
    {
        [Key]
        public int OrderLineItemId { get; set; }
        public int OrderId { get; set; }
        public string ItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Manufacturer { get; set; }
        public int Qty { get; set; }
        public decimal? Price { get; set; }
        public decimal? SubTotal { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [JsonIgnore] // Circular reference prevention
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
