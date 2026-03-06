using Basket.API.Data;
using Basket.API.Endpoints;
using Basket.API.GrpcServices;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddGrpcClient<CatalogProtoService.CatalogProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:CatalogUrl"]!);
})
.AddStandardResilienceHandler();

builder.Services.AddScoped<CatalogGrpcClient>();

builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapBasketEndpoints();
app.MapHealthChecks("/health");

app.Run();
