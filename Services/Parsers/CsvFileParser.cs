using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;
using CsvHelper;
using CsvHelper.Configuration;

namespace SalesOrderManagement.Services.Parsers
{
    public class CsvFileParser : IFileParser
    {
        public Task<PurchaseOrderResponseDto> ParseAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            using var csv = new CsvReader(reader, config);
            
            var records = csv.GetRecords<PurchaseOrderItemDto>().ToList();

            return Task.FromResult(new PurchaseOrderResponseDto
            {
                Items = records,
                DetectedType = "CSV",
                Success = true
            });
        }
    }
}
