using BCrypt.Net;
using MediatR;
using MediClaim.Application.Common.Exceptions;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Common;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using System.Text.Json;

namespace MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;

public class RegisterTenantCommandHandler
    : IRequestHandler<
        RegisterTenantCommand,
        Guid>
{
    private readonly ITenantRepository
        _tenantRepository;

    private readonly IUserRepository
        _userRepository;

    private readonly IUnitOfWork
        _unitOfWork;
    private readonly IEncryptionService
    _encryptionService;

    public RegisterTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService)
    {
        _tenantRepository = tenantRepository;

        _userRepository = userRepository;

        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
    }

    public async Task<Guid> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        var slugExists =
            await _tenantRepository
                .SlugExistsAsync(request.Slug);

        if (slugExists)
        {
            throw new ConflictException(
                "Tenant slug already exists");
        }

        var user =
            await _userRepository
                .EmailExistsAsync(
                    request.AdminEmail);

        if (user is not null)
        {
            throw new ConflictException(
                "Admin email already exists");
        }
        var settings =
    new TenantSettings
    {
        FraudThreshold =
            request.FraudThreshold
    };
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),

            Name = request.TenantName,

            Slug = request.Slug,

            Status = TenantStatus.Active,
            SettingsJson =
            JsonSerializer.Serialize(
                settings)
        };

        await _tenantRepository
            .AddAsync(tenant);

        var adminUser = new User
        {
            UserId = Guid.NewGuid(),

            TenantId = tenant.TenantId,

            Email = request.AdminEmail,

            PasswordHash = BCrypt.Net.BCrypt
                .HashPassword(request.Password),
            SsnEncrypted =
        _encryptionService.Encrypt(
            request.Ssn),

            Role = UserRole.TenantAdmin
        };

        await _userRepository
            .AddAsync(adminUser);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return tenant.TenantId;
    }
}