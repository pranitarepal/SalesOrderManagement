using System;
using System.Collections.Generic;

namespace SalesOrderManagement.Models.DTOs
{
    public class OrderDetailsDto
    {
        //public OrderInfoDto OrderInfo { get; set; }
        public int OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public List<OrderItemDto> ItemDetails { get; set; } = new List<OrderItemDto>();
    }

    public class OrderListDto
    {
        public List<OrderDetailsDto> Orders { get; set; } = new List<OrderDetailsDto>();
    }

    //public class OrderInfoDto
    //{
    //    public int OrderId { get; set; }
    //    public string OrderStatus { get; set; }
    //}

    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public string? ProductName { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? Price { get; set; }
        public int? Qty { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
