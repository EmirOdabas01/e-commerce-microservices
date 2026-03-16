using Carter;
using MediatR;

namespace Catalog.API.Features.GetProductById;

public record GetProductByIdResponse(Guid Id, string Name, List<string> Category, string Description, string ImageFile, List<string> ImageFiles, decimal Price);

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetProductById")
        .Produces<GetProductByIdResult>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Product By Id");
    }
}
