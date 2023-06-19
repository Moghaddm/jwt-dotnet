namespace Auth.Jwt.Services.User;
public class UserService : IUserService
{
    public async Task<string> GetUserRole(string userName)
    {
        await Task.CompletedTask;
        return "admin";
        throw new NotImplementedException();
    }

    public async Task<bool> IsValidUserCredentials(string userName, string password)
    {
        await Task.CompletedTask;
        return true;
    }
}