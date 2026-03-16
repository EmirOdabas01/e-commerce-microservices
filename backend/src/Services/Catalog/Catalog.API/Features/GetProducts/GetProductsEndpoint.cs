using Carter;
using MediatR;

namespace Catalog.API.Features.GetProducts;

public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10, decimal? MinPrice = null, decimal? MaxPrice = null, string? SortBy = null, string? SortOrder = null);
public record GetProductsResponse(IEnumerable<ProductDto> Data, long Count, int PageIndex, int PageSize);
public record ProductDto(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
        {
            var query = new GetProductsQuery(
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortOrder);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .Produces<GetProductsResult>()
        .WithSummary("Get Products");
    }
}
