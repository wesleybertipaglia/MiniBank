using Microsoft.EntityFrameworkCore;

namespace MiniBank.Auth.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}