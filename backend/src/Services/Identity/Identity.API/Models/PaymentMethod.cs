namespace Identity.API.Models;

public class PaymentMethod
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string CardName { get; set; } = default!;
    public string CardNumberLast4 { get; set; } = default!;
    public string Expiration { get; set; } = default!;
    public bool IsDefault { get; set; }
}
