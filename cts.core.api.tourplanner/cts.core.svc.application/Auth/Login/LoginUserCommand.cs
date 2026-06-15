namespace cts.core.svc.application.Auth.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password);