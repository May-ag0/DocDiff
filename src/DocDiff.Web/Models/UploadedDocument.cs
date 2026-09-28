namespace DocDiff.Web.Models;

public record UploadedDocument(string FileName, long SizeInBytes, string Content);
