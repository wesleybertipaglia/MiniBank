using Microsoft.EntityFrameworkCore;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Model;
using MiniBank.Auth.Infrastructure.Data;

namespace MiniBank.Auth.Infrastructure.Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> GetByIdAsync(Guid id)
    { 
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var entity = context.Users.Update(user);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task DeleteAsync(User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}