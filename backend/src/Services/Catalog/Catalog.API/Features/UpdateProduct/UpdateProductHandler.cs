using BuildingBlocks.Exceptions;
using Catalog.API.Models;
using FluentValidation;
using Marten;
using MediatR;

namespace Catalog.API.Features.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : IRequest<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IDocumentSession _session;

    public UpdateProductHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), command.Id);
        }

        product.Name = command.Name;
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        _session.Update(product);
        await _session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
