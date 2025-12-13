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
                VendorName = processedResponse.VendorName,
                VendorAddress = processedResponse.VendorAddress,
                PurchaseOrderNumber = processedResponse.PurchaseOrderNumber,
                PurchaseOrderDate = processedResponse.PurchaseOrderDate,
                Currency = processedResponse.Currency,
                Subtotal = processedResponse.Subtotal,
                Tax = processedResponse.Tax,
                GrandTotal = processedResponse.GrandTotal,
                SourceFileName = file.FileName,
                LineItems = processedResponse.Items.Select(i => new PurchaseOrderLineItem
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity ?? 0,
                    UnitPrice = i.Price,
                    Brand = i.Brand,
                    Total = i.Total
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
    }
}
