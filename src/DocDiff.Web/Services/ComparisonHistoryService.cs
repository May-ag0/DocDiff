using DocDiff.Web.Data;
using DocDiff.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DocDiff.Web.Services;

/// <summary>
/// Stores and reads comparison metadata. Each call creates its own short-lived DbContext,
/// which is the recommended pattern for Blazor Server.
/// </summary>
public class ComparisonHistoryService(IDbContextFactory<AppDbContext> dbFactory) : IComparisonHistoryService
{
    public async Task SaveAsync(DocumentComparison comparison)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Comparisons.Add(comparison);
        await db.SaveChangesAsync();
    }

    public async Task<List<DocumentComparison>> GetRecentAsync(int count)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Comparisons
            .AsNoTracking()
            .OrderByDescending(c => c.ComparedAt)
            .Take(count)
            .ToListAsync();
    }
}
