using System.Threading.Tasks;

namespace SalesOrderManagement.Services.AI
{
    public interface IAIService
    {
        Task<string> ExtractJsonAsync(string systemPrompt, string userContent);
    }
}
