using System.Security.Claims;
using Auth.Jwt.Models;

namespace Auth.Jwt.Services;
public interface IJwtAuthenticationManager
{
    JwtAuthenticationResult GenerateTokens(string userName,Claim[] claims,DateTime now);
    Task RemoveRefreshTokenByUserName(string userName);
    JwtAuthenticationResult Refresh(string refreshToken,string accessToken,DateTime now);
}