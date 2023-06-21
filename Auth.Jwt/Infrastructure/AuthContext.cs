using Auth.Jwt.Domain;
using Microsoft.EntityFrameworkCore;

namespace Auth.Jwt.Infrastructure;
public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        
    }
}