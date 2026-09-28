using DocDiff.Web.Services;

namespace DocDiff.Tests;

public class UploadValidatorTests
{
    [Theory]
    [InlineData("contract.txt")]
    [InlineData("notes.md")]
    [InlineData("UPPERCASE.TXT")]
    public void ValidateFile_SupportedExtension_ReturnsNull(string fileName)
    {
        Assert.Null(UploadValidator.ValidateFile(fileName, 100));
    }

    [Theory]
    [InlineData("contract.pdf")]
    [InlineData("contract.docx")]
    [InlineData("no-extension")]
    public void ValidateFile_UnsupportedExtension_ReturnsError(string fileName)
    {
        Assert.NotNull(UploadValidator.ValidateFile(fileName, 100));
    }

    [Fact]
    public void ValidateFile_ExactlyMaxSize_ReturnsNull()
    {
        Assert.Null(UploadValidator.ValidateFile("a.txt", UploadValidator.MaxFileSizeBytes));
    }

    [Fact]
    public void ValidateFile_LargerThanMaxSize_ReturnsError()
    {
        Assert.NotNull(UploadValidator.ValidateFile("a.txt", UploadValidator.MaxFileSizeBytes + 1));
    }

    [Fact]
    public void ValidateContent_NormalText_ReturnsNull()
    {
        Assert.Null(UploadValidator.ValidateContent("a.txt", "Hello\nWorld"));
    }

    [Fact]
    public void ValidateContent_BinaryContent_ReturnsError()
    {
        Assert.NotNull(UploadValidator.ValidateContent("a.txt", "PK\0\u0003binary"));
    }

    [Fact]
    public void ValidateContent_AtLineLimit_ReturnsNull()
    {
        var content = string.Join("\n", Enumerable.Repeat("line", UploadValidator.MaxLineCount));

        Assert.Null(UploadValidator.ValidateContent("a.txt", content));
    }

    [Fact]
    public void ValidateContent_OverLineLimit_ReturnsError()
    {
        var content = string.Join("\n", Enumerable.Repeat("line", UploadValidator.MaxLineCount + 1));

        Assert.NotNull(UploadValidator.ValidateContent("a.txt", content));
    }
}
