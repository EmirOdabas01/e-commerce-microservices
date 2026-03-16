using Catalog.API.Models;
using FluentValidation;
using Marten;
using MediatR;

namespace Catalog.API.Features.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, List<string> ImageFiles, decimal Price, int Stock, string SellerId)
    : IRequest<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.ImageFiles).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IDocumentSession _session;

    public CreateProductHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFiles = command.ImageFiles,
            Price = command.Price,
            Stock = command.Stock,
            SellerId = command.SellerId
        };

        _session.Store(product);
        await _session.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
