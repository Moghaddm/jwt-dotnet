using Auth.Jwt.Enums;

namespace Auth.Jwt.Models;

public record RegisterRequest(string FirstName, string LastName, string UserName, string Password,UserRoles userRole = UserRoles.user);
