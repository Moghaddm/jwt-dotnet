namespace Auth.Jwt.Models;
public class JwtAuthenticationResult
{
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    
}