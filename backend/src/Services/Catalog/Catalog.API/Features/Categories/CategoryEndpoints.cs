using Carter;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Features.Categories;

public record CategoryRequest(string Name, string? Description);
public record CategoryResponse(Guid Id, string Name, string? Description, DateTime CreatedAt);

public class CategoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async (IDocumentSession session) =>
        {
            var categories = await session.Query<Category>()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Results.Ok(categories.Select(c =>
                new CategoryResponse(c.Id, c.Name, c.Description, c.CreatedAt)));
        })
        .WithName("GetCategories");

        app.MapPost("/api/categories", async (CategoryRequest request, IDocumentSession session) =>
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            session.Store(category);
            await session.SaveChangesAsync();

            return Results.Created($"/api/categories/{category.Id}",
                new CategoryResponse(category.Id, category.Name, category.Description, category.CreatedAt));
        })
        .WithName("CreateCategory")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        app.MapPut("/api/categories/{id:guid}", async (Guid id, CategoryRequest request, IDocumentSession session) =>
        {
            var category = await session.LoadAsync<Category>(id);
            if (category is null) return Results.NotFound();

            category.Name = request.Name;
            category.Description = request.Description;

            session.Update(category);
            await session.SaveChangesAsync();

            return Results.Ok(new CategoryResponse(category.Id, category.Name, category.Description, category.CreatedAt));
        })
        .WithName("UpdateCategory")
        .RequireAuthorization(p => p.RequireRole("Admin"));

        app.MapDelete("/api/categories/{id:guid}", async (Guid id, IDocumentSession session) =>
        {
            var category = await session.LoadAsync<Category>(id);
            if (category is null) return Results.NotFound();

            session.Delete(category);
            await session.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
