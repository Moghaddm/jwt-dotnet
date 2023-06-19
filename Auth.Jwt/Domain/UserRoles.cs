using Microsoft.AspNetCore.Identity;

namespace Auth.Jwt.Domain;
public class UserRoles : IdentityRole
{
    public List<User> Users { get; set; }
}