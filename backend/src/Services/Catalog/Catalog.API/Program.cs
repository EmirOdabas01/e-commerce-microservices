using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions;
using Carter;
using Catalog.API.GrpcServices;
using FluentValidation;
using Marten;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCarter();
builder.Services.AddGrpc();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapCarter();
app.MapGrpcService<CatalogGrpcService>();
app.MapHealthChecks("/health");

app.Run();
