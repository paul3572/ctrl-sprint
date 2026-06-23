namespace cts.core.svc.contracts.Auth;

public sealed class AuthResponse
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;
    
    public DateTime CreatedAt { get; init; }

    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
}