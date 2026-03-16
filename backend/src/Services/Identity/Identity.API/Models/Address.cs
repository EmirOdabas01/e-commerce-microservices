namespace Identity.API.Models;

public class Address
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string State { get; set; } = default!;
    public string ZipCode { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
    public bool IsDefault { get; set; }
}
