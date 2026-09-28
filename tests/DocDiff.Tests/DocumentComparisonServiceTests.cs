using DocDiff.Web.Models;
using DocDiff.Web.Services;

namespace DocDiff.Tests;

public class DocumentComparisonServiceTests
{
    private readonly DocumentComparisonService _service = new();

    private static string Lines(params string[] lines) => string.Join("\n", lines);

    [Fact]
    public void Compare_IdenticalDocuments_AllLinesUnchanged()
    {
        var text = Lines("First line", "Second line", "Third line");

        var result = _service.Compare(text, text);

        Assert.All(result.Lines, line => Assert.Equal(DiffLineType.Unchanged, line.Type));
        Assert.Equal(3, result.UnchangedCount);
        Assert.False(result.HasChanges);
    }

    [Fact]
    public void Compare_LineAdded_MarksOnlyThatLineAsAdded()
    {
        var original = Lines("A", "B", "C");
        var updated = Lines("A", "B", "New line", "C");

        var result = _service.Compare(original, updated);

        DiffLine[] expected =
        [
            new(DiffLineType.Unchanged, "A", 1, 1),
            new(DiffLineType.Unchanged, "B", 2, 2),
            new(DiffLineType.Added, "New line", null, 3),
            new(DiffLineType.Unchanged, "C", 3, 4),
        ];
        Assert.Equal(expected, result.Lines);
    }

    [Fact]
    public void Compare_LineRemoved_MarksOnlyThatLineAsRemoved()
    {
        var original = Lines("A", "B", "C");
        var updated = Lines("A", "C");

        var result = _service.Compare(original, updated);

        DiffLine[] expected =
        [
            new(DiffLineType.Unchanged, "A", 1, 1),
            new(DiffLineType.Removed, "B", 2, null),
            new(DiffLineType.Unchanged, "C", 3, 2),
        ];
        Assert.Equal(expected, result.Lines);
    }

    [Fact]
    public void Compare_LineChanged_ShowsRemovedBeforeAdded()
    {
        var original = Lines("Payment within 30 days.");
        var updated = Lines("Payment within 14 days.");

        var result = _service.Compare(original, updated);

        DiffLine[] expected =
        [
            new(DiffLineType.Removed, "Payment within 30 days.", 1, null),
            new(DiffLineType.Added, "Payment within 14 days.", null, 1),
        ];
        Assert.Equal(expected, result.Lines);
    }

    [Fact]
    public void Compare_OriginalEmpty_AllLinesAdded()
    {
        var result = _service.Compare("", Lines("A", "B"));

        Assert.All(result.Lines, line => Assert.Equal(DiffLineType.Added, line.Type));
        Assert.Equal(2, result.AddedCount);
    }

    [Fact]
    public void Compare_UpdatedEmpty_AllLinesRemoved()
    {
        var result = _service.Compare(Lines("A", "B"), "");

        Assert.All(result.Lines, line => Assert.Equal(DiffLineType.Removed, line.Type));
        Assert.Equal(2, result.RemovedCount);
    }

    [Fact]
    public void Compare_BothEmpty_ReturnsNoLines()
    {
        var result = _service.Compare("", "");

        Assert.Empty(result.Lines);
        Assert.False(result.HasChanges);
    }

    [Theory]
    [InlineData("A\r\nB\r\nC")] // Windows
    [InlineData("A\nB\nC\n")]   // trailing newline
    public void Compare_LineEndingDifferences_AreIgnored(string original)
    {
        var result = _service.Compare(original, "A\nB\nC");

        Assert.False(result.HasChanges);
        Assert.Equal(3, result.UnchangedCount);
    }

    [Fact]
    public void Compare_MultipleChanges_CountsAreCorrect()
    {
        var original = Lines("Title", "Old paragraph", "Shared", "Removed line");
        var updated = Lines("Title", "New paragraph", "Shared", "Added line", "Another added line");

        var result = _service.Compare(original, updated);

        Assert.Equal(3, result.AddedCount);
        Assert.Equal(2, result.RemovedCount);
        Assert.Equal(2, result.UnchangedCount);
    }
}
