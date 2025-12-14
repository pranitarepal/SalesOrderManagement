using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderManagement.Models.Entities
{
    [Table("ItemMaster")]
    public class ItemMaster
    {
        [Key]
        public int ItemId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal Price { get; set; }

        public string Desc { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int AvailableQty { get; set; }

        public string Manufacturer { get; set; }
    }
}
