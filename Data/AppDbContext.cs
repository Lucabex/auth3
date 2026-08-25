using Microsoft.EntityFrameworkCore;
using auth3.Models;
namespace auth3.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User>Users{get; set;}
}