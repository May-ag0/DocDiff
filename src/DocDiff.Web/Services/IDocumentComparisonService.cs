using DocDiff.Web.Models;

namespace DocDiff.Web.Services;

public interface IDocumentComparisonService
{
    ComparisonResult Compare(string originalText, string updatedText);
}
