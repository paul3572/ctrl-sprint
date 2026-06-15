using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Abstractions.Time;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.contracts.Users;

namespace cts.core.svc.application.Auth.Register;

public sealed class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHashingService passwordHashingService,
        IAccessTokenGenerator accessTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashingService = passwordHashingService;
        _accessTokenGenerator = accessTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthenticationResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        ValidatePassword(command.Password);

        string normalizedEmail = User.NormalizeEmail(command.Email);

        bool userAlreadyExists = await _userRepository.ExistsByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (userAlreadyExists)
        {
            throw new EmailAlreadyRegisteredException();
        }

        string passwordHash = _passwordHashingService.HashPassword(command.Password);

        User user = User.Create(
            command.Email,
            command.DisplayName,
            passwordHash,
            _dateTimeProvider.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        AccessToken accessToken = _accessTokenGenerator.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.Email,
            user.DisplayName,
            accessToken.Value,
            accessToken.ExpiresAtUtc);
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new PasswordPolicyViolationException("Password must not be empty.");
        }

        if (password.Length < 8)
        {
            throw new PasswordPolicyViolationException("Password must contain at least 8 characters.");
        }
    }
}