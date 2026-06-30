using cts.core.svc.application.Auth;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.application.Auth.Login;
using cts.core.svc.application.Auth.Register;
using cts.core.svc.application.Services;
using cts.core.svc.contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterAuthService _registerAuthService;
    private readonly LoginAuthService _loginAuthService;

    public AuthController(
        RegisterAuthService registerAuthService,
        LoginAuthService loginAuthService)
    {
        _registerAuthService = registerAuthService;
        _loginAuthService = loginAuthService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            AuthenticationResult result = await _registerAuthService.HandleAsync(
                new RegisterUserCommand(
                    request.Email,
                    request.Password),
                cancellationToken);
            
            this.SetAuthCookie(result);

            return Ok(ToResponse(result));
        }
        catch (DbUpdateException exception)
        {
            if (exception.InnerException is PostgresException pg)
            {
                return pg.SqlState switch
                {
                    "23505" => Conflict(new ProblemDetails
                    {
                        Title = "Duplicate key",
                        Status = 409,
                        Detail = "Email already exists."
                    }),
                    "23503" => Conflict(new ProblemDetails
                    {
                        Title = "Foreign key violation",
                        Status = 409,
                        Detail = "Referenced entity does not exist."
                    }),

                    _ => Problem(detail: pg.MessageText)
                };
            }

            return Problem(detail: exception.Message);
        }
        catch (PasswordPolicyViolationException exception)
        {
            return this.Problem(detail: "Password needs to be at least 8 characters long.");
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            AuthenticationResult result = await _loginAuthService.HandleAsync(
                new LoginUserCommand(
                    request.Email,
                    request.Password),
                cancellationToken);
            
            this.SetAuthCookie(result);

            return Ok(ToResponse(result));
        }
        catch (InvalidCredentialsException exception)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid credentials",
                Detail = exception.Message
            });
        }
    }
    
    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthResponse> GetMe()
    {
        var userGuidClaim = this.User.FindFirst("sub")?.Value;
        var emailClaim = this.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userGuidClaim is null || emailClaim is null)
        {
            return Unauthorized();
        }

        return Ok(new AuthResponse
        {
            UserGuid = Guid.Parse(userGuidClaim),
            Email = emailClaim,
            CreatedAt = default,
            AccessToken = string.Empty,
            AccessTokenExpiresAtUtc = default
        });
    }

    private static AuthResponse ToResponse(AuthenticationResult result)
    {
        return new AuthResponse
        {
            UserGuid = result.UserGuid,
            Email = result.Email,
            CreatedAt = result.CreatedAt,
            AccessToken = result.AccessToken,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc
        };
    }
    
    private void SetAuthCookie(AuthenticationResult result)
    {
        Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // only dev
            SameSite = SameSiteMode.Lax,
            Expires = result.AccessTokenExpiresAtUtc.UtcDateTime
        });
    }
}