using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.Services.Parsers
{
    public class XmlFileParser : IFileParser
    {
        public Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(List<PurchaseOrderItemDto>));
            var items = (List<PurchaseOrderItemDto>)serializer.Deserialize(stream);

            return Task.FromResult(new PurchaseOrderResponseDto
            {
                Items = items ?? new List<PurchaseOrderItemDto>(),
                DetectedType = "XML",
                Success = true
            });
        }
    }
}
