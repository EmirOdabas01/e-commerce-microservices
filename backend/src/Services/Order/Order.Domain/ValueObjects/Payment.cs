namespace Order.Domain.ValueObjects;

public record Payment(
    string CardName,
    string CardNumber,
    string Expiration,
    string Cvv,
    int PaymentMethod);
