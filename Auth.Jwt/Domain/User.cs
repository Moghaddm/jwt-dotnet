using Microsoft.AspNetCore.Identity;

namespace Auth.Jwt.Domain;

public class User : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }

    public User(string firstName, string lastName, string userName, string password,UserRole role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentNullException(firstName);
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentNullException(lastName);
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentNullException(userName);
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(password);
        FirstName = firstName;
        LastName = lastName;
        UserName = userName;
        Password = password;
        Role = role;
    }
    public User()
    {
        
    }
    public UserRole Role { get; set; }
}
