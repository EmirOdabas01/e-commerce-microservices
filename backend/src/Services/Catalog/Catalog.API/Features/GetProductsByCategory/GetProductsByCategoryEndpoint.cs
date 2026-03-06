using Carter;
using MediatR;

namespace Catalog.API.Features.GetProductsByCategory;

public class GetProductsByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/category/{category}", async (string category, ISender sender) =>
        {
            var result = await sender.Send(new GetProductsByCategoryQuery(category));
            return Results.Ok(result);
        })
        .WithName("GetProductsByCategory")
        .Produces<GetProductsByCategoryResult>()
        .WithSummary("Get Products By Category");
    }
}
