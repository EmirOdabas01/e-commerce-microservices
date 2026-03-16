using Catalog.API.Models;
using Marten;
using Marten.Pagination;
using MediatR;

namespace Catalog.API.Features.GetProducts;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SortBy = null,
    string? SortOrder = null) : IRequest<GetProductsResult>;

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
        var queryable = _session.Query<Product>().AsQueryable();

        if (query.MinPrice.HasValue)
            queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);

        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "price" => query.SortOrder?.ToLowerInvariant() == "desc"
                ? queryable.OrderByDescending(p => p.Price)
                : queryable.OrderBy(p => p.Price),
            "name" => query.SortOrder?.ToLowerInvariant() == "desc"
                ? queryable.OrderByDescending(p => p.Name)
                : queryable.OrderBy(p => p.Name),
            _ => queryable
        };

        var products = await queryable.ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new GetProductsResult(products, products.TotalItemCount, query.PageNumber - 1, query.PageSize);
    }
}
