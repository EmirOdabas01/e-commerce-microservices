using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging;
using Notification.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapHealthChecks("/health");

app.MapGet("/api/notifications/status", () => Results.Ok(new { Status = "Notification service is running" }));

app.Run();
