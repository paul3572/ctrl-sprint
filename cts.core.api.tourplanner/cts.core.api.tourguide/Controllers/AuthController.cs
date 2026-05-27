using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Application.Auth;
using TourPlanner.Application.Auth.Exceptions;
using TourPlanner.Application.Auth.Login;
using TourPlanner.Application.Auth.Register;
using TourPlanner.Contracts.Auth;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;

    public AuthController(
        RegisterUserCommandHandler registerUserCommandHandler,
        LoginUserCommandHandler loginUserCommandHandler)
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            AuthenticationResult result = await _registerUserCommandHandler.HandleAsync(
                new RegisterUserCommand(
                    request.Email,
                    request.DisplayName,
                    request.Password),
                cancellationToken);

            return Ok(ToResponse(result));
        }
        catch (EmailAlreadyRegisteredException exception)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Email already registered",
                Detail = exception.Message
            });
        }
        catch (PasswordPolicyViolationException exception)
        {
            return BadRequest(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    ["password"] = new[] { exception.Message }
                })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Password policy violation",
                Detail = exception.Message
            });
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
            AuthenticationResult result = await _loginUserCommandHandler.HandleAsync(
                new LoginUserCommand(
                    request.Email,
                    request.Password),
                cancellationToken);

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

    private static AuthResponse ToResponse(AuthenticationResult result)
    {
        return new AuthResponse
        {
            UserId = result.UserId,
            Email = result.Email,
            DisplayName = result.DisplayName,
            AccessToken = result.AccessToken,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc
        };
    }
}