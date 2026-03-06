namespace Basket.API.GrpcServices;

public class CatalogGrpcClient
{
    private readonly CatalogProtoService.CatalogProtoServiceClient _client;

    public CatalogGrpcClient(CatalogProtoService.CatalogProtoServiceClient client)
    {
        _client = client;
    }

    public async Task<GetProductResponse> GetProduct(Guid productId)
    {
        var request = new GetProductRequest { Id = productId.ToString() };
        return await _client.GetProductAsync(request);
    }
}
