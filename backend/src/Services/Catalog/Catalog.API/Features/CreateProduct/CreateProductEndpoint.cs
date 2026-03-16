using System.Security.Claims;
using Carter;
using Mapster;
using MediatR;

namespace Catalog.API.Features.CreateProduct;

public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, List<string>? ImageFiles, decimal Price, int Stock = 0);
public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", async (CreateProductRequest request, ISender sender, HttpContext context) =>
        {
            var sellerId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var imageFiles = request.ImageFiles ?? (string.IsNullOrEmpty(request.ImageFile) ? [] : [request.ImageFile]);
            var command = new CreateProductCommand(request.Name, request.Category, request.Description, imageFiles, request.Price, request.Stock, sellerId);
            var result = await sender.Send(command);
            var response = result.Adapt<CreateProductResponse>();
            return Results.Created($"/api/products/{response.Id}", response);
        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .RequireAuthorization(p => p.RequireRole("Seller", "Admin"));
    }
}
