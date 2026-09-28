using DocDiff.Web.Models;

namespace DocDiff.Web.Services;

/// <summary>
/// Line-based diff using the Longest Common Subsequence (LCS) algorithm.
/// Lines that are part of the LCS are unchanged; the rest are removed or added.
/// </summary>
public class DocumentComparisonService : IDocumentComparisonService
{
    public ComparisonResult Compare(string originalText, string updatedText)
    {
        var original = SplitLines(originalText);
        var updated = SplitLines(updatedText);
        var lcs = BuildLcsTable(original, updated);

        var lines = new List<DiffLine>();
        int i = 0, j = 0;

        while (i < original.Length && j < updated.Length)
        {
            if (original[i] == updated[j])
            {
                lines.Add(new DiffLine(DiffLineType.Unchanged, original[i], i + 1, j + 1));
                i++;
                j++;
            }
            else if (lcs[i + 1, j] >= lcs[i, j + 1])
            {
                // Skipping the original line keeps the longest common subsequence intact.
                lines.Add(new DiffLine(DiffLineType.Removed, original[i], i + 1, null));
                i++;
            }
            else
            {
                lines.Add(new DiffLine(DiffLineType.Added, updated[j], null, j + 1));
                j++;
            }
        }

        for (; i < original.Length; i++)
            lines.Add(new DiffLine(DiffLineType.Removed, original[i], i + 1, null));

        for (; j < updated.Length; j++)
            lines.Add(new DiffLine(DiffLineType.Added, updated[j], null, j + 1));

        return new ComparisonResult(lines);
    }

    // table[i, j] = length of the LCS of original[i..] and updated[j..]
    private static int[,] BuildLcsTable(string[] original, string[] updated)
    {
        var table = new int[original.Length + 1, updated.Length + 1];

        for (int i = original.Length - 1; i >= 0; i--)
        {
            for (int j = updated.Length - 1; j >= 0; j--)
            {
                table[i, j] = original[i] == updated[j]
                    ? table[i + 1, j + 1] + 1
                    : Math.Max(table[i + 1, j], table[i, j + 1]);
            }
        }

        return table;
    }

    private static string[] SplitLines(string text)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var lines = text.ReplaceLineEndings("\n").Split('\n');

        // A trailing newline should not count as an extra empty line.
        return lines[^1] == "" ? lines[..^1] : lines;
    }
}
