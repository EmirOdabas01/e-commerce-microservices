using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Order.API.Endpoints;
using Order.Infrastructure;
using Order.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Order.Application.Commands.CreateOrder.CreateOrderCommand).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Order.Application.Commands.CreateOrder.CreateOrderCommand).Assembly);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapOrderEndpoints();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
