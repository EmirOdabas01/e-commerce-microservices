using Carter;
using MediatR;

namespace Catalog.API.Features.DeleteProduct;

public record DeleteProductResponse(bool IsSuccess);

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/products/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id));
            return Results.Ok(new DeleteProductResponse(result.IsSuccess));
        })
        .WithName("DeleteProduct")
        .Produces<DeleteProductResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Product");
    }
}
