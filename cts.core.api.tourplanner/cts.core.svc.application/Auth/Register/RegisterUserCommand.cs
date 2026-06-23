namespace cts.core.svc.application.Auth.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password);