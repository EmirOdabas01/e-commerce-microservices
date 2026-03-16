namespace Catalog.API.Models;

public class ReviewReport
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public string ReportedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
