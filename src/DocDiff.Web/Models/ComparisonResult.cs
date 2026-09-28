namespace DocDiff.Web.Models;

public record ComparisonResult(IReadOnlyList<DiffLine> Lines)
{
    public int AddedCount => Lines.Count(l => l.Type == DiffLineType.Added);
    public int RemovedCount => Lines.Count(l => l.Type == DiffLineType.Removed);
    public int UnchangedCount => Lines.Count(l => l.Type == DiffLineType.Unchanged);
    public bool HasChanges => AddedCount > 0 || RemovedCount > 0;
}
