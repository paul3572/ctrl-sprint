using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Time;
using cts.core.svc.contracts.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace cts.core.svc.infrastructure.Authentication;

internal sealed class JwtAccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtAccessTokenGenerator(
        IOptions<JwtOptions> jwtOptions,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public AccessToken Generate(User user)
    {
        DateTimeOffset expiresAtUtc = _dateTimeProvider.UtcNow.AddMinutes(
            _jwtOptions.AccessTokenExpirationMinutes);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName)
        ];

        SymmetricSecurityKey securityKey = new(
            Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        SigningCredentials signingCredentials = new(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: _dateTimeProvider.UtcNow.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(tokenValue, expiresAtUtc);
    }
}