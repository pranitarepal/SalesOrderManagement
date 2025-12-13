using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using UglyToad.PdfPig;

namespace SalesOrderManagement.Services.Parsers
{
    public class PdfFileParser : IFileParser
    {
        public Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var items = new List<PurchaseOrderItemDto>();
            
            // PdfPig needs a seekable stream usually, or we load into memory
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            using var document = PdfDocument.Open(ms);
            foreach (var page in document.GetPages())
            {
                var text = page.Text;
                // Very naive parsing: assumption is each line might be an item
                // Implement logic to parse "ItemId, ItemName..." from text text
                // For this demo, we'll try to split by newline and naive comma separation or just return raw text in a generic item if format is unknown
                
                var lines = text.Split('\n');
                foreach (var line in lines)
                {
                   // Placeholder logic: Real PDF parsing requires layout analysis
                   // We will just skip for now or try to identify lines with 6 parts
                   var parts = line.Split(' ');
                   if (parts.Length >= 6)
                   {
                        // Demo logic
                        // items.Add(...)
                   }
                }
            }

            return Task.FromResult(new PurchaseOrderResponseDto
            {
                Items = items,
                DetectedType = "PDF",
                Success = true
            });
        }
    }
}
