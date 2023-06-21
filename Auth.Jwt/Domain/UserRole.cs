using Auth.Jwt.Enums;
using Microsoft.AspNetCore.Identity;

namespace Auth.Jwt.Domain;

public class UserRole : Entity
{
    private UserRoles _role;
    public string Role
    {
        private set { }
        get =>
            _ = _role switch
            {
                UserRoles.admin => "admin",
                UserRoles.user => "user"
            };
    }

    public UserRole() { }

    public UserRole(UserRoles role) => _role = role;

    public List<User> Users { get; set; }
}
