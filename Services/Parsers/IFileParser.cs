using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.Services.Parsers
{
    public interface IFileParser
    {
        Task<PurchaseOrderResponseDto> ParseAsync(Stream stream);
    }
}
