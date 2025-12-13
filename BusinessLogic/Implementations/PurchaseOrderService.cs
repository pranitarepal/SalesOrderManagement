using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.DataAccess;
using SalesOrderManagement.Models.DTOs;
using SalesOrderManagement.Models.Entities;
using SalesOrderManagement.Services;

namespace SalesOrderManagement.BusinessLogic.Implementations
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly FileParserFactory _parserFactory;
        private readonly SalesOrderDbContext _context;

        public PurchaseOrderService(FileParserFactory parserFactory, SalesOrderDbContext context)
        {
            _parserFactory = parserFactory;
            _context = context;
        }

        public async Task<PurchaseOrderApiResponseDto> ProcessPurchaseOrderAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var parser = _parserFactory.GetParser(file.FileName, stream);

            if (parser == null)
            {
                throw new ArgumentException($"Unsupported file type: {Path.GetExtension(file.FileName)}");
            }

            var processedResponse = await parser.ParseAsync(stream);
            processedResponse.SourceFileName = file.FileName;
            processedResponse.DetectedType = parser.GetType().Name;

            // Map DTO to Entity
            var purchaseOrder = new PurchaseOrder
            {
                //VendorName = processedResponse.VendorName,
                //VendorAddress = processedResponse.VendorAddress,
                //PurchaseOrderNumber = processedResponse.PurchaseOrderNumber,
                //PurchaseOrderDate = processedResponse.PurchaseOrderDate,
                //Currency = processedResponse.Currency,
                //Subtotal = processedResponse.Subtotal,
                //Tax = processedResponse.Tax,
                //GrandTotal = processedResponse.GrandTotal,
                //SourceFileName = file.FileName,
                LineItems = processedResponse.Items.Select(i => new PurchaseOrderLineItem
                {
                    ProductName = i.ItemName,
                    Qty = i.Quantity ?? 0,
                    Price = i.Price,
                    Manufacturer = i.Brand,
                    SubTotal = i.Total
                }).ToList()
            };

            // Save to Database
            _context.PurchaseOrders.Add(purchaseOrder);
            //await _context.SaveChangesAsync();

            // Return Restricted API DTO
            return new PurchaseOrderApiResponseDto
            {
                SourceFileName = processedResponse.SourceFileName,
                DetectedType = processedResponse.DetectedType,
                Success = processedResponse.Success,
                ErrorMessage = processedResponse.ErrorMessage,
                OrderLineItems = processedResponse.Items.Select(i => new PurchaseOrderApiItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Brand = i.Brand
                }).ToList()
            };
        }
        public async Task<PurchaseOrderApiResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequestDto request)
        {
            // Map DTO to Entity
            var purchaseOrder = new PurchaseOrder
            {
                // Manually created, so no specific parsed meta-data other than user input
                //DetectedType = "Manual Entry", 
                //SourceFileName = "Manual Entry",
                UserId = request.Customer?.UserId,
                CreatedDate = DateTime.UtcNow,
                LineItems = request.OrderLineItems?.Select(i => new PurchaseOrderLineItem
                {
                    ItemId = i.ItemId?.ToString(),
                    ProductName = i.ItemName,
                    Qty = i.Quantity,
                    Price = i.Price,
                    Manufacturer = i.Manufacturer,
                    SubTotal = (decimal?)i.Quantity * (i.Price ?? 0) // Basic calc if needed
                }).ToList() ?? new List<PurchaseOrderLineItem>()
            };

            // Calculate totals
            purchaseOrder.TotalAmount = purchaseOrder.LineItems.Sum(li => li.SubTotal);
            //purchaseOrder.GrandTotal = purchaseOrder.Subtotal; // Simple logic for now
            try
            {
                // Save to Database
                _context.PurchaseOrders.Add(purchaseOrder);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error saving purchase order to database: " + ex.Message);
            }

            // Return DTO
            return new PurchaseOrderApiResponseDto
            {
                SourceFileName = "Manual Entry",
                DetectedType = "Manual Entry",
                Success = true,
                OrderLineItems = purchaseOrder.LineItems.Select(i => new PurchaseOrderApiItemDto
                {
                    ItemName = i.ProductName,
                    Quantity = i.Qty,
                    Brand = i.Manufacturer,
                    Price = i.Price
                }).ToList()
            };
        }
    }
}
