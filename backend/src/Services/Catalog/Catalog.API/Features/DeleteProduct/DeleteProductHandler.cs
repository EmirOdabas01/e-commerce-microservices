using BuildingBlocks.Exceptions;
using Catalog.API.Models;
using Marten;
using MediatR;

namespace Catalog.API.Features.DeleteProduct;

public record DeleteProductCommand(Guid Id, string SellerId, bool IsAdmin) : IRequest<DeleteProductResult>;
public record DeleteProductResult(bool IsSuccess);

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly IDocumentSession _session;

    public DeleteProductHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), command.Id);
        }

        if (!command.IsAdmin && product.SellerId != command.SellerId)
        {
            throw new UnauthorizedAccessException("You can only delete your own products.");
        }

        _session.Delete(product);
        await _session.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}
