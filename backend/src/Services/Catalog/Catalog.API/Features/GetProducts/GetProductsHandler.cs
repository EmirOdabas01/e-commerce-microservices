using Catalog.API.Models;
using Mapster;
using Marten;
using Marten.Pagination;
using MediatR;

namespace Catalog.API.Features.GetProducts;

public record GetProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<GetProductsResult>;
public record GetProductsResult(IEnumerable<Product> Data, long Count, int PageIndex, int PageSize);

public class GetProductsHandler : IRequestHandler<GetProductsQuery, GetProductsResult>
{
    private readonly IDocumentSession _session;

    public GetProductsHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _session.Query<Product>()
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new GetProductsResult(products, products.TotalItemCount, query.PageNumber - 1, query.PageSize);
    }
}
