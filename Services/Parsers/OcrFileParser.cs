using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using Tesseract;

namespace SalesOrderManagement.Services.Parsers
{
    public class OcrFileParser : IFileParser
    {
        public Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var items = new List<PurchaseOrderItemDto>();
            
            // Tesseract requires a byte array or file
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var imageBytes = ms.ToArray();

            try 
            {
                using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);
                
                var text = page.GetText();
                // Naive parsing of extracted text
                // Similar to PDF, we would parse 'text' to find items
            }
            catch (Exception ex)
            {
                // Log error or handle missing tessdata
                Console.WriteLine($"OCR Error: {ex.Message}");
            }

            return Task.FromResult(new PurchaseOrderResponseDto
            {
                Items = items,
                DetectedType = "Image (OCR)",
                Success = true
            });
        }
    }
}
