namespace cts.core.svc.application.Abstractions.Authentication;

public sealed record AccessToken(
    string Value,
    DateTimeOffset ExpiresAtUtc);