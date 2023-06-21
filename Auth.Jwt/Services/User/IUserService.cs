using Auth.Jwt.Models;

namespace Auth.Jwt.Services.User;
public interface IUserService 
{
    Task<bool> IsValidUserCredentials(string userName,string password);
    Task<string> GetUserRole(string userName);
    Task<long> RegisterUser(RegisterRequest request);
    Task<bool> IsExist(string user);
}