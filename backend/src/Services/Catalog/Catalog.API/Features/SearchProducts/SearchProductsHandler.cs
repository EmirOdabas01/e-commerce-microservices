using Catalog.API.Models;
using Marten;
using Marten.Pagination;
using MediatR;

namespace Catalog.API.Features.SearchProducts;

public record SearchProductsQuery(string Query, int PageNumber = 1, int PageSize = 10) : IRequest<SearchProductsResult>;
public record SearchProductsResult(IEnumerable<Product> Data, long Count, int PageIndex, int PageSize);

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, SearchProductsResult>
{
    private readonly IDocumentSession _session;

    public SearchProductsHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<SearchProductsResult> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var searchTerm = query.Query.ToLowerInvariant();

        var products = await _session.Query<Product>()
            .Where(p => p.Name.ToLower().Contains(searchTerm) || p.Description.ToLower().Contains(searchTerm))
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new SearchProductsResult(products, products.TotalItemCount, query.PageNumber - 1, query.PageSize);
    }
}
