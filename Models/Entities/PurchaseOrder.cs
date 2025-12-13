using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesOrderManagement.Models.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string Currency { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Tax { get; set; }
        public decimal? GrandTotal { get; set; }
        public string SourceFileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<PurchaseOrderLineItem> LineItems { get; set; } = new List<PurchaseOrderLineItem>();
    }
}
