using Carter;
using MediatR;

namespace Catalog.API.Features.SearchProducts;

public record SearchProductsRequest(string Query, int? PageNumber = 1, int? PageSize = 10);

public class SearchProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/search", async ([AsParameters] SearchProductsRequest request, ISender sender) =>
        {
            var query = new SearchProductsQuery(request.Query, request.PageNumber ?? 1, request.PageSize ?? 10);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("SearchProducts")
        .Produces<SearchProductsResult>()
        .WithSummary("Search Products");
    }
}
