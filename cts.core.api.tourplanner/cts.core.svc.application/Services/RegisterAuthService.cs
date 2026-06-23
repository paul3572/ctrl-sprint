using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.contracts;

namespace cts.core.svc.application.Auth.Register;

public sealed class RegisterAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public RegisterAuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _accessTokenGenerator = accessTokenGenerator;
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

        User user = User.Create(
            command.Email,
            command.Password,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        AccessToken accessToken = _accessTokenGenerator.Generate(user);

        return new AuthenticationResult(
            user.UserGuid,
            user.Email,
            user.CreatedAtUtc,
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