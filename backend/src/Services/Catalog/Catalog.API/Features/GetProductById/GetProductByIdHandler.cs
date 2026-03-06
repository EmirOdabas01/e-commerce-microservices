using BuildingBlocks.Exceptions;
using Catalog.API.Models;
using Marten;
using MediatR;

namespace Catalog.API.Features.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<GetProductByIdResult>;
public record GetProductByIdResult(Product Product);

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResult>
{
    private readonly IDocumentSession _session;

    public GetProductByIdHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _session.LoadAsync<Product>(query.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), query.Id);
        }

        return new GetProductByIdResult(product);
    }
}
