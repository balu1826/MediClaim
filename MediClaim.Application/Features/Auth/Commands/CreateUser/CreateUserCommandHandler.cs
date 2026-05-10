using BCrypt.Net;
using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<
        CreateUserCommand,
        Guid>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly IEncryptionService
        _encryptionService;

    private readonly IUserRepository
        _currentUserService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService,
        IUserRepository currentUserService)
    {
        _userRepository =
            userRepository;

        _unitOfWork =
            unitOfWork;

        _encryptionService =
            encryptionService;

        _currentUserService =
            currentUserService;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser =
            await _userRepository
                .EmailExistsAsync(
                    request.Email);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "Email already exists");
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),

            TenantId =
                _currentUserService
                    .TenantId,

            Email = request.Email,

            PasswordHash =
                BCrypt.Net.BCrypt
                    .HashPassword(
                        request.Password),

            SsnEncrypted =
                _encryptionService
                    .Encrypt(
                        request.Ssn),

            Role = request.Role,

            ApprovalLimit =
                request.ApprovalLimit,

            IsFraudSpecialist =
                request.IsFraudSpecialist
        };

        await _userRepository
            .AddAsync(user);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return user.UserId;
    }
}