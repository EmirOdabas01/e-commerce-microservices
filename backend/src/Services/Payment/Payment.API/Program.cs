using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapHealthChecks("/health");

app.MapGet("/api/payment/status", () => Results.Ok(new { Status = "Payment service is running" }));

app.Run();
