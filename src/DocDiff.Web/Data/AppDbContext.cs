using DocDiff.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DocDiff.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DocumentComparison> Comparisons => Set<DocumentComparison>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var comparison = modelBuilder.Entity<DocumentComparison>();

        comparison.Property(c => c.OriginalFileName).HasMaxLength(255);
        comparison.Property(c => c.UpdatedFileName).HasMaxLength(255);

        // SQLite has no date type, so the DateTimeKind is lost when reading.
        // We always store UTC, so mark values as UTC when they are loaded.
        comparison.Property(c => c.ComparedAt)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
