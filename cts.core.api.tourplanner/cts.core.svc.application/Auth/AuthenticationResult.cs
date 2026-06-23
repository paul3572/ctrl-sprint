namespace cts.core.svc.application.Auth;

public sealed record AuthenticationResult(
    Guid UserGuid,
    string Email,
    DateTime CreatedAt,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
    