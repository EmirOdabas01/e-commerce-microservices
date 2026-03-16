using System.Security.Claims;
using Carter;
using Mapster;
using MediatR;

namespace Catalog.API.Features.UpdateProduct;

public record UpdateProductRequest(Guid Id, string Name, List<string> Category, string Description, string ImageFile, List<string>? ImageFiles, decimal Price);
public record UpdateProductResponse(bool IsSuccess);

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/products", async (UpdateProductRequest request, ISender sender, HttpContext context) =>
        {
            var sellerId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = context.User.IsInRole("Admin");
            var imageFiles = request.ImageFiles ?? (string.IsNullOrEmpty(request.ImageFile) ? [] : [request.ImageFile]);
            var command = new UpdateProductCommand(request.Id, request.Name, request.Category, request.Description, imageFiles, request.Price, sellerId, isAdmin);
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateProductResponse>();
            return Results.Ok(response);
        })
        .WithName("UpdateProduct")
        .Produces<UpdateProductResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Product")
        .RequireAuthorization(p => p.RequireRole("Seller", "Admin"));
    }
}
