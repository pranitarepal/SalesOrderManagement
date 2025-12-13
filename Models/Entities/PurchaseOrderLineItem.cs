using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.Entities
{
    public class PurchaseOrderLineItem
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public string ItemName { get; set; }
        public string Brand { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Total { get; set; }

        [JsonIgnore] // Circular reference prevention
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
