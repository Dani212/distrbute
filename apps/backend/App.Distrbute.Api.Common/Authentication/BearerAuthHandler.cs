using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Common;
using App.Distrbute.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace App.Distrbute.Api.Common.Authentication;

public class BearerAuthHandler(IOptions<BearerTokenConfig> bearerTokenConfig)
{
    private readonly BearerTokenConfig _bearerTokenConfig = bearerTokenConfig.Value;

    public (string token, long expiresMillis) GenerateJwtToken(Email user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_bearerTokenConfig.SigningKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.GivenName, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Address)
            }, CommonConstants.AuthScheme.BEARER),
            Issuer = _bearerTokenConfig.Issuer,
            Expires = DateTime.UtcNow.AddDays(7),
            Audience = _bearerTokenConfig.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        var expiryMillis = new DateTimeOffset(tokenDescriptor.Expires.Value).ToUnixTimeMilliseconds();

        return (tokenString, expiryMillis);
    }
}