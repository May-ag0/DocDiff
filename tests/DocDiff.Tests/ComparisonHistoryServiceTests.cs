using DocDiff.Web.Data;
using DocDiff.Web.Models;
using DocDiff.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DocDiff.Tests;

/// <summary>
/// Integration tests against a real SQLite database that lives in memory for the duration of each test.
/// </summary>
public class ComparisonHistoryServiceTests : IDisposable
{
    // An in-memory SQLite database exists only while its connection is open.
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly ComparisonHistoryService _service;

    public ComparisonHistoryServiceTests()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var db = new AppDbContext(options))
            db.Database.Migrate();

        _service = new ComparisonHistoryService(new TestDbContextFactory(options));
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetRecentAsync_ReturnsNewestFirst()
    {
        var now = DateTime.UtcNow;
        await _service.SaveAsync(NewComparison("old.txt", now.AddHours(-2)));
        await _service.SaveAsync(NewComparison("newest.txt", now));
        await _service.SaveAsync(NewComparison("middle.txt", now.AddHours(-1)));

        var result = await _service.GetRecentAsync(10);

        Assert.Equal(["newest.txt", "middle.txt", "old.txt"], result.Select(c => c.OriginalFileName));
    }

    [Fact]
    public async Task GetRecentAsync_RespectsCount()
    {
        for (var i = 0; i < 5; i++)
            await _service.SaveAsync(NewComparison($"file{i}.txt", DateTime.UtcNow.AddMinutes(i)));

        var result = await _service.GetRecentAsync(3);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task SaveAsync_ComparedAtIsReadBackAsUtc()
    {
        var comparedAt = new DateTime(2026, 9, 28, 12, 0, 0, DateTimeKind.Utc);
        await _service.SaveAsync(NewComparison("a.txt", comparedAt));

        var saved = Assert.Single(await _service.GetRecentAsync(1));

        Assert.Equal(DateTimeKind.Utc, saved.ComparedAt.Kind);
        Assert.Equal(comparedAt, saved.ComparedAt);
    }

    private static DocumentComparison NewComparison(string originalFileName, DateTime comparedAt) => new()
    {
        OriginalFileName = originalFileName,
        UpdatedFileName = "updated.txt",
        ComparedAt = comparedAt,
        AddedCount = 1,
        RemovedCount = 1
    };

    private class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }
}
