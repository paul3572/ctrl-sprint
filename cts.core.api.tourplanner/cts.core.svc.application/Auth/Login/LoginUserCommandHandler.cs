using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.contracts;

namespace cts.core.svc.application.Auth.Login;

public sealed class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<AuthenticationResult> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        string normalizedEmail = User.NormalizeEmail(command.Email);

        User user = await _userRepository.GetByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken) ?? throw new InvalidCredentialsException();

        bool passwordIsValid = _passwordHashingService.VerifyPassword(
            user.PasswordHash,
            command.Password);

        if (!passwordIsValid)
        {
            throw new InvalidCredentialsException();
        }

        AccessToken accessToken = _accessTokenGenerator.Generate(user);

        return new AuthenticationResult(
            user.UserGuid,
            user.Email,
            user.CreatedAtUtc,
            accessToken.Value,
            accessToken.ExpiresAtUtc);
    }
}