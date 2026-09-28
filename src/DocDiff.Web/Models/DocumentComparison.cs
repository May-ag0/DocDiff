namespace DocDiff.Web.Models;

public class DocumentComparison
{
    public int Id { get; set; }
    public required string OriginalFileName { get; set; }
    public required string UpdatedFileName { get; set; }
    public DateTime ComparedAt { get; set; }
    public int AddedCount { get; set; }
    public int RemovedCount { get; set; }
}
