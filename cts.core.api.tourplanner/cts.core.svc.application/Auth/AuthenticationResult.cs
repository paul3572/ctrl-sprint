namespace cts.core.svc.application.Auth;

public sealed record AuthenticationResult(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
    