using Microsoft.EntityFrameworkCore;
using MiniBank.Auth.Core.Model;

namespace MiniBank.Auth.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}