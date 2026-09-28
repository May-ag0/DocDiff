namespace DocDiff.Web.Models;

public record DiffLine(
    DiffLineType Type,
    string Text,
    int? OriginalLineNumber,
    int? UpdatedLineNumber);
