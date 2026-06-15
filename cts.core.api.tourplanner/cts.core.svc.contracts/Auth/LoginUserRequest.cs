using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.contracts.Auth;

public sealed class LoginUserRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; init; } = string.Empty;
}