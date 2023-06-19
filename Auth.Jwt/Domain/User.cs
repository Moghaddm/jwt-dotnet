using Microsoft.AspNetCore.Identity;

namespace Auth.Jwt.Domain;
public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}