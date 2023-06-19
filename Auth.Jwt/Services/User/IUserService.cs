namespace Auth.Jwt.Services.User;
public interface IUserService 
{
    Task<bool> IsValidUserCredentials(string userName,string password);
    Task<string> GetUserRole(string userName);
}