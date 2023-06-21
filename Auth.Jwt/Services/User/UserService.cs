using Auth.Jwt.Infrastructure;
using Auth.Jwt.Models;

namespace Auth.Jwt.Services.User;

public class UserService : IUserService
{
    private readonly AuthContext _context;

    public UserService(AuthContext context) => _context = context;

    public async Task<long> RegisterUser(RegisterRequest request)
    {
        if (request.UserName is null || request.Password is null)
            throw new ApplicationException("Username or Password Not Valid.");
        if (await IsExist(request.UserName))
            throw new ApplicationException($"{request.UserName} UserName is Taken.");
        await _context.Users.AddAsync(
            new Domain.User(request.FirstName, request.LastName, request.UserName, request.Password,new Domain.UserRole(request.userRole))
        );
        return await _context.SaveChangesAsync();
    }

    public async Task<string> GetUserRole(string userName)
    {
        await Task.CompletedTask;
        var userRole = _context.Users.FirstOrDefault(user => user.UserName == userName)!.Role.Role;
        if (string.IsNullOrEmpty(userRole))
            throw new KeyNotFoundException("User Not Found.");
        return userRole;
    }

    public async Task<bool> IsValidUserCredentials(string userName, string password)
    {
        await Task.CompletedTask;
        var user = _context.Users.FirstOrDefault(
            user => user.UserName == userName && user.Password == password
        );
        if (user is null)
            return false;
        return true;
    }

    public async Task<bool> IsExist(string userName)
    {
        await Task.CompletedTask;
        bool exist = _context.Users.FirstOrDefault(user => user.UserName == userName) is not null
            ? true
            : false;
        return exist;
    }
}
