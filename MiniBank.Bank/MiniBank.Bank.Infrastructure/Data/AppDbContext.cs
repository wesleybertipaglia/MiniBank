using Microsoft.EntityFrameworkCore;

namespace MiniBank.Bank.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}