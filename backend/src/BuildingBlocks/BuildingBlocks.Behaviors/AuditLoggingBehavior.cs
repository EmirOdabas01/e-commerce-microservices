using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

public class AuditLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuditLoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLoggingBehavior(ILogger<AuditLoggingBehavior<TRequest, TResponse>> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var userName = user?.FindFirstValue(ClaimTypes.Name) ?? "anonymous";
        var isAdmin = user?.IsInRole("Admin") ?? false;
        var requestName = typeof(TRequest).Name;

        if (isAdmin && (requestName.Contains("Create") || requestName.Contains("Update") || requestName.Contains("Delete") || requestName.Contains("Cancel") || requestName.Contains("Refund")))
        {
            _logger.LogInformation(
                "AUDIT: User {UserId} ({UserName}) executed {RequestName} with {@Request}",
                userId, userName, requestName, request);
        }

        var response = await next(cancellationToken);

        return response;
    }
}
