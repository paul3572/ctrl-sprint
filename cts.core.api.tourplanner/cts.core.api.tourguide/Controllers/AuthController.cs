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
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RegisterAuthService registerAuthService,
        LoginAuthService loginAuthService,
        ILogger<AuthController> logger)
    {
        _registerAuthService = registerAuthService;
        _loginAuthService = loginAuthService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration attempt for {Email}", request.Email);
        
        try
        {
            AuthenticationResult result = await _registerAuthService.HandleAsync(
                new RegisterUserCommand(
                    request.Email,
                    request.Password),
                cancellationToken);
            
            SetAuthCookie(result);
            
            _logger.LogInformation(
                "User {Email} registered successfully. UserGuid={UserGuid}",
                result.Email,
                result.UserGuid);
            return Ok(ToResponse(result));
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception,
                "Database error while registering user {Email}",
                request.Email);
            
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
        catch (PasswordPolicyViolationException)
        {
            _logger.LogWarning(
                "Password policy violation during registration for {Email}",
                request.Email);
            
            return Problem(detail: "Password needs to be at least 8 characters long.");
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);
        
        try
        {
            AuthenticationResult result = await _loginAuthService.HandleAsync(
                new LoginUserCommand(
                    request.Email,
                    request.Password),
                cancellationToken);
            
            this.SetAuthCookie(result);

            _logger.LogInformation("User {Email} logged in successfully", result.Email);
            return Ok(ToResponse(result));
        }
        catch (InvalidCredentialsException exception)
        {
            _logger.LogWarning(
                "Failed login attempt for {Email}",
                request.Email);
            
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
        _logger.LogInformation("Authorization attempt.");
        
        var userGuidClaim =
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var emailClaim =
            User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userGuidClaim is null || emailClaim is null)
            return Unauthorized();

        _logger.LogInformation("User {Email} logged in successfully.", emailClaim);
        
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
            AccessToken = string.Empty,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc,
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