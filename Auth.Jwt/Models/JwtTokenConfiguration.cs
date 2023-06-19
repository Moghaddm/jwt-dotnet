namespace Auth.Jwt.Models;
public class JwtTokenConfiguration
{
    public string Secret { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}