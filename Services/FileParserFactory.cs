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
                ".csv" => new CsvFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".xml" => new XmlFileParser(),
                ".xlsx" => new ExcelFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".xls" => new ExcelFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".pdf" => new PdfFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".docx" => new WordFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".doc" => new WordFileParser(_serviceProvider.GetRequiredService<IAIService>()), // Note: OpenXML might fail on old binary .doc, but trying anyway or assuming robust converter handling in future
                ".png" or ".jpg" or ".jpeg" or ".tiff" => new OcrFileParser(),
                ".txt" => new CsvFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".msg" => new EmailFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                ".eml" => new EmailFileParser(_serviceProvider.GetRequiredService<IAIService>()),
                _ => null
            };
        }
    }
}
