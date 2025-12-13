using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderManagement.Models.Entities
{
    [Table("Order")]
    public class PurchaseOrder
    {
        [Key]
        public int OrderId { get; set; }
        //public string VendorName { get; set; }
        //public string VendorAddress { get; set; }
        //public string PurchaseOrderNumber { get; set; }
        //public DateTime? PurchaseOrderDate { get; set; }
        //public string Currency { get; set; }
        public decimal? TotalAmount { get; set; }
        //public decimal? Tax { get; set; }
        //public decimal? GrandTotal { get; set; }
        //public string SourceFileName { get; set; }
        //public string DetectedType { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public string OrderStatus { get; set; }
        public string Notes { get; set; }

        public List<PurchaseOrderLineItem> LineItems { get; set; } = new List<PurchaseOrderLineItem>();
    }
}
