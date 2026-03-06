using Catalog.API.Models;
using Marten;
using MediatR;

namespace Catalog.API.Features.GetProductsByCategory;

public record GetProductsByCategoryQuery(string Category) : IRequest<GetProductsByCategoryResult>;
public record GetProductsByCategoryResult(IEnumerable<Product> Products);

public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    private readonly IDocumentSession _session;

    public GetProductsByCategoryHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        var products = await _session.Query<Product>()
            .Where(p => p.Category.Contains(query.Category))
            .ToListAsync(cancellationToken);

        return new GetProductsByCategoryResult(products);
    }
}
