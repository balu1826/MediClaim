using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MediClaim.Application
    .Common.Behaviours;

public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    public AuditBehaviour(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        ICurrentTenantService currentTenantService,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUserService = currentUserService;
        _currentTenantService = currentTenantService;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)

    {
        var response = await next();
        if (request is IAuditableCommand auditableRequest)
        {
            var httpContext =
                _httpContextAccessor
                    .HttpContext;

            var correlationId =
                httpContext?
                    .Items[
                        "X-Correlation-ID"]
                            ?.ToString();

            var ipAddress =
                httpContext?
                    .Connection
                    .RemoteIpAddress
                    ?.ToString();

            var auditLog =
                new AuditLog
                {
                    TenantId =
                        _currentTenantService
                            .TenantId
                                ?? Guid.Empty,

                    EntityType =
                        auditableRequest
                            .EntityType,

                    EntityId =
                        auditableRequest
                            .EntityId,

                    Action =
                        auditableRequest
                            .Action,

                    ChangedByUserId =
                        _currentUserService
                            .UserId,

                    CorrelationId =
                        correlationId,

                    IpAddress =
                        ipAddress,

                    Timestamp =
                        DateTime.UtcNow
                };

            await _context.AuditLogs
                .AddAsync(
                    auditLog,
                    cancellationToken);

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);
        }

        return response;
    }
}