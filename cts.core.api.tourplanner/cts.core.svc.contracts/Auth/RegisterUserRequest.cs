using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.contracts.Auth;

public sealed class RegisterUserRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(200)]
    public string Password { get; init; } = string.Empty;
}