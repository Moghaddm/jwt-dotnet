using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Jwt.Models;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Jwt.Services;

public class JwtAuthenticationManager : IJwtAuthenticationManager
{
    private IImmutableDictionary<string, RefreshToken> UserRefreshTokensReadOnlyDictionary =>
        _userRefreshToken.ToImmutableDictionary();
    private readonly byte[] _secret;
    private readonly ConcurrentDictionary<string, RefreshToken> _userRefreshToken;
    private readonly JwtTokenConfiguration _jwtTokenConfiguration;

    public JwtAuthenticationManager(JwtTokenConfiguration jwtTokenConfiguration)
    {
        _jwtTokenConfiguration = jwtTokenConfiguration;
        _userRefreshToken = new ConcurrentDictionary<string, RefreshToken>();
        _secret = Encoding.ASCII.GetBytes(jwtTokenConfiguration.Secret);
    }

    public JwtAuthenticationResult GenerateTokens(string userName, Claim[] claims, DateTime now)
    {
        var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(
            claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value
        );

        var jwtToken = new JwtSecurityToken(
            _jwtTokenConfiguration.Issuer,
            shouldAddAudienceClaim ? _jwtTokenConfiguration.Audience : string.Empty,
            claims,
            expires: now.AddMinutes(_jwtTokenConfiguration.AccessTokenExpiration),
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new SymmetricSecurityKey(_secret),
                SecurityAlgorithms.HmacSha256Signature
            )
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var refreshToken = new RefreshToken
        {
            UserName = userName,
            ExpireTime = now.AddMinutes(_jwtTokenConfiguration.RefreshTokenExpiration),
            Token = GenerateRefreshTokenString()
        };
        _userRefreshToken.AddOrUpdate(refreshToken.Token, refreshToken, (s, t) => refreshToken);
        return new JwtAuthenticationResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task RemoveRefreshTokenByUserName(string userName)
    {
        var keysToRemove = _userRefreshToken.Keys
            .Where(k => _userRefreshToken[k].UserName == userName)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _userRefreshToken.TryRemove(key, out _);
        }
        await Task.CompletedTask;
    }

    private static string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public JwtAuthenticationResult Refresh(string refreshToken, string accessToken, DateTime now)
    {
        var (principal, jwtToken) = DecodeJwtToken(accessToken);
        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userName = principal.Identity.Name;
        if (!_userRefreshToken.TryGetValue(refreshToken, out var existingRefreshToken))
        {
            throw new SecurityTokenException("Invalid token");
        }
        if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireTime < now)
        {
            throw new SecurityTokenException("Invalid token");
        }

        return GenerateTokens(userName, principal.Claims.ToArray(), now);
    }

    public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SecurityTokenException("Invalid token");
        }
        var principal = new JwtSecurityTokenHandler().ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtTokenConfiguration.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secret),
                ValidAudience = _jwtTokenConfiguration.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            },
            out var validatedToken
        );
        return (principal, validatedToken as JwtSecurityToken);
    }
}
