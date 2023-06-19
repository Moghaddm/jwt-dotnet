namespace Auth.Jwt.Models;
public class RefreshToken
{
    public string UserName { get; set; }
    public string Token { get; set; }
    public DateTime ExpireTime { get; set; }

}