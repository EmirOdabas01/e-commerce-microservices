namespace Catalog.API.Models;

public class Review
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public int Rating { get; set; }
    public string Text { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
