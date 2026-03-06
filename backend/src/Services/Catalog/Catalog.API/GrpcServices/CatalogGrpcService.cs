using Catalog.API.Models;
using Grpc.Core;
using Marten;

namespace Catalog.API.GrpcServices;

public class CatalogGrpcService : CatalogProtoService.CatalogProtoServiceBase
{
    private readonly IDocumentSession _session;

    public CatalogGrpcService(IDocumentSession session)
    {
        _session = session;
    }

    public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = await _session.LoadAsync<Product>(Guid.Parse(request.Id));

        if (product is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID {request.Id} not found"));
        }

        var response = new GetProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            ImageFile = product.ImageFile,
            Price = (double)product.Price
        };

        response.Category.AddRange(product.Category);

        return response;
    }
}
