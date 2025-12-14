using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

            // Validate required fields and check ItemMaster
            var validItems = new List<PurchaseOrderItemDto>();
            var invalidItemsWithErrors = new List<(PurchaseOrderItemDto item, List<string> errors)>();

            // Get all item names from ItemMaster for comparison
            var itemMasterNames = await _context.ItemMasters
                .Select(im => im.ProductName.ToLower())
                .ToListAsync();

            foreach (var item in processedResponse.Items)
            {
                var errors = new List<string>();
                
                // Check if required fields are present
                bool hasItemName = !string.IsNullOrWhiteSpace(item.ItemName);
                bool hasQuantity = item.Quantity.HasValue && item.Quantity.Value > 0;
                bool hasManufacturer = !string.IsNullOrWhiteSpace(item.Brand);

                // Validate required fields
                if (!hasItemName)
                    errors.Add("ItemName is missing");
                
                if (!item.Quantity.HasValue)
                    errors.Add("Quantity is missing");
                else if (item.Quantity.Value <= 0)
                    errors.Add("Quantity is invalid");
                
                if (!hasManufacturer)
                    errors.Add("Manufacturer is missing");

                // If no validation errors, check ItemMaster
                if (errors.Count == 0)
                {
                    bool existsInMaster = itemMasterNames.Contains(item.ItemName.ToLower());
                    
                    if (!existsInMaster)
                    {
                        errors.Add("Item not found in Item Master");
                    }
                }

                // Categorize item
                if (errors.Count > 0)
                {
                    invalidItemsWithErrors.Add((item, errors));
                }
                else
                {
                    validItems.Add(item);
                }
            }

            // Map only valid items (that exist in ItemMaster) to Entity for database storage
            var purchaseOrder = new PurchaseOrder
            {
                UserId = null,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                OrderStatus = "Pending",
                Notes = $"Uploaded from file: {file.FileName}",
                OrderItems = validItems.Select(i => new OrderItem
                {
                    ProductName = i.ItemName,
                    Qty = i.Quantity ?? 0,
                    Price = i.Price,
                    Manufacturer = i.Brand,
                    Subtotal = i.Total
                }).ToList()
            };

            // Calculate total amount
            purchaseOrder.TotalAmount = purchaseOrder.OrderItems.Sum(oi => oi.Subtotal);

            // Save to Database only if there are valid items
            if (validItems.Any())
            {
                _context.Orders.Add(purchaseOrder);
                //await _context.SaveChangesAsync();
            }

            // Build detailed error/warning message
            var errorMessage = processedResponse.ErrorMessage;
            
            if (invalidItemsWithErrors.Any())
            {
                var errorDetails = invalidItemsWithErrors.Select(tuple => 
                {
                    var (item, errors) = tuple;
                    var itemIdentifier = !string.IsNullOrWhiteSpace(item.ItemName) 
                        ? $"Item '{item.ItemName}'" 
                        : "Unknown Item";
                    return $"{itemIdentifier}: {string.Join(", ", errors)}";
                });

                var warningMessage = $"Invalid items ({invalidItemsWithErrors.Count}): {string.Join("; ", errorDetails)}";
                errorMessage = string.IsNullOrEmpty(errorMessage) 
                    ? warningMessage 
                    : $"{errorMessage} | {warningMessage}";
            }

            // Return response with separated lists
            return new PurchaseOrderApiResponseDto
            {
                SourceFileName = processedResponse.SourceFileName,
                DetectedType = processedResponse.DetectedType,
                Success = processedResponse.Success && validItems.Any(),
                ErrorMessage = errorMessage,
                OrderLineItems = validItems.Select(i => new PurchaseOrderApiItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Brand = i.Brand,
                    Price = i.Price
                }).ToList(),
                InvalidItems = invalidItemsWithErrors.Select(tuple => 
                {
                    var (item, errors) = tuple;
                    return new InvalidItemDetailsDto
                    {
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        Manufacturer = item.Brand,
                        Price = item.Price,
                        ErrorMessage = string.Join(", ", errors)
                    };
                }).ToList()
            };
        }

        public async Task<PurchaseOrderApiResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequestDto request)
        {
            // Map DTO to Entity
            var purchaseOrder = new PurchaseOrder
            {
                UserId = request.Customer?.UserId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                OrderStatus = "InProgress",
                Notes = request.Notes,
                OrderItems = request.OrderLineItems.Select(i => new OrderItem
                {
                    ItemId = i.ItemId,
                    ProductName = i.ItemName,
                    Qty = i.Quantity,
                    Price = i.Price,
                    Manufacturer = i.Manufacturer,
                    Subtotal = (decimal?)i.Quantity * i.Price
                }).ToList()
            };

            // Calculate totals
            purchaseOrder.TotalAmount = purchaseOrder.OrderItems.Sum(li => li.Subtotal);
            
            try
            {
                // Save to Database
                _context.Orders.Add(purchaseOrder);
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
                OrderLineItems = purchaseOrder.OrderItems.Select(i => new PurchaseOrderApiItemDto
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
