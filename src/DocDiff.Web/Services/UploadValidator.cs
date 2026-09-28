namespace DocDiff.Web.Services;

/// <summary>
/// Validation rules for uploaded documents. Returns an error message, or null if the input is valid.
/// </summary>
public static class UploadValidator
{
    public const long MaxFileSizeBytes = 1024 * 1024; // 1 MB

    // The diff algorithm uses memory proportional to lines × lines, so we cap the line count.
    public const int MaxLineCount = 5_000;

    public static readonly string[] AllowedExtensions = [".txt", ".md"];

    public static string? ValidateFile(string fileName, long sizeInBytes)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return $"\"{fileName}\" is not supported. Only .txt and .md files are allowed.";

        if (sizeInBytes > MaxFileSizeBytes)
            return $"\"{fileName}\" is larger than 1 MB.";

        return null;
    }

    public static string? ValidateContent(string fileName, string content)
    {
        if (content.Contains('\0'))
            return $"\"{fileName}\" does not look like a text file.";

        var lineCount = content.Count(c => c == '\n') + 1;
        if (lineCount > MaxLineCount)
            return $"\"{fileName}\" has more than {MaxLineCount:N0} lines.";

        return null;
    }
}
