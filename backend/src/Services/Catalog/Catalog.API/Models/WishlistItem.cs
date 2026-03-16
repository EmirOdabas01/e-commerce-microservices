namespace Catalog.API.Models;

public class WishlistItem
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public Guid ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
