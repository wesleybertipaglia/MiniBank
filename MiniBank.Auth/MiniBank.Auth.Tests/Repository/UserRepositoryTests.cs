using Microsoft.EntityFrameworkCore;
using MiniBank.Auth.Core.Model;
using MiniBank.Auth.Infrastructure.Data;
using MiniBank.Auth.Infrastructure.Repository;

namespace MiniBank.Auth.Tests.Repository;

public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_AddsUserToDatabase()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashedpassword"
        };

        var result = await _repository.CreateAsync(user);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Single(_context.Users);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashedpassword"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashedpassword"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync(user.Email);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserInDatabase()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Email = "update@example.com",
            Password = "hashedpassword"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        user.Name = "New Name";
        var result = await _repository.UpdateAsync(user);

        Assert.Equal("New Name", result.Name);

        var fromDb = await _context.Users.FindAsync(user.Id);
        Assert.Equal("New Name", fromDb.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserFromDatabase()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "ToDelete",
            Email = "delete@example.com",
            Password = "hashedpassword"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(user);

        var fromDb = await _context.Users.FindAsync(user.Id);
        Assert.Null(fromDb);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}