using Carter;
using MediatR;

namespace Catalog.API.Features.GetProducts;

public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);
public record GetProductsResponse(IEnumerable<ProductDto> Products);
public record ProductDto(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
        {
            var query = new GetProductsQuery(request.PageNumber ?? 1, request.PageSize ?? 10);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .Produces<GetProductsResult>()
        .WithSummary("Get Products");
    }
}
