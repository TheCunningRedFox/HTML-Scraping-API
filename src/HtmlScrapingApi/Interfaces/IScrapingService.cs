using TestAssignment.Models;

namespace TestAssignment.Interfaces
{
    public interface IScrapingService
    {
        Task<ScrapingResponse> Process(ScrapingRequest request, CancellationToken ct);
    }
}
