using FluentValidation;

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.AddressLine).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.CardName).NotEmpty();
        RuleFor(x => x.CardNumber).NotEmpty();
    }
}
