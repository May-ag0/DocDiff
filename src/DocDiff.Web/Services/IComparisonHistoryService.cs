using DocDiff.Web.Models;

namespace DocDiff.Web.Services;

public interface IComparisonHistoryService
{
    Task SaveAsync(DocumentComparison comparison);
    Task<List<DocumentComparison>> GetRecentAsync(int count);
}
