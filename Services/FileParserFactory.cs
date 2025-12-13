using System;
using System.IO;
using SalesOrderManagement.Services.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SalesOrderManagement.Services.AI;

namespace SalesOrderManagement.Services
{
    public class FileParserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FileParserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileParser GetParser(string fileName, Stream stream)
        {
            var extension = Path.GetExtension(fileName).ToLower();

            // Simple extension based first, could be enhanced with magic number check
            return extension switch
            {
                ".json" => new JsonFileParser(),
                ".csv" => new CsvFileParser(),
                ".xml" => new XmlFileParser(),
                ".xlsx" => new ExcelFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".xls" => new ExcelFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".pdf" => new PdfFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".png" or ".jpg" or ".jpeg" or ".tiff" => new OcrFileParser(),
                ".txt" => new CsvFileParser(), // Treat txt as CSV/TSV for now or implement TextParser
                _ => null
            };
        }
    }
}
