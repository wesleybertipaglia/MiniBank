using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Helper;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;

namespace MiniBank.Auth.Application.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBrokerPublisher _messageBrokerPublisher;
    private readonly ILogger<UserService> _logger;
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IUserRepository userRepository,
        IMessageBrokerPublisher messageBrokerPublisher,
        ILogger<UserService> logger,
        ICacheService cacheService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _messageBrokerPublisher = messageBrokerPublisher;
        _logger = logger;
        _cacheService = cacheService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var cacheKey = $"user:{id}";
        
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);
        if (cachedUser != null)
        {
            LogHelper.LogInfo(_logger, "[CACHE HIT] User found in cache for ID {UserId}", [id]);
            return cachedUser;
        }

        LogHelper.LogInfo(_logger, "Fetching user by ID: {UserId}", [id]);
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            LogHelper.LogWarning(_logger, "User with ID {UserId} not found.", [id]);
            throw new Exception($"User with ID '{id}' not found.");
        }

        LogHelper.LogInfo(_logger, "User found: {UserId}", [id]);

        var userDto = UserMapper.Map(user);
        
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(5));

        return userDto;
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var cacheKey = $"user:email:{email}";
        
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);
        if (cachedUser != null)
        {
            LogHelper.LogInfo(_logger, "[CACHE HIT] User found in cache for Email {Email}", [email]);
            return cachedUser;
        }

        LogHelper.LogInfo(_logger, "Fetching user by email: {Email}", [email]);
        
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            LogHelper.LogWarning(_logger, "User with email {Email} not found.", [email]);
            throw new Exception($"User with email '{email}' not found.");
        }

        LogHelper.LogInfo(_logger, "User found with email: {Email}", [email]);

        var userDto = UserMapper.Map(user);
        
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(5));

        return userDto;
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        var userId = GetUserIdFromToken();

        if (userId != Guid.Empty)
            return await GetByIdAsync(userId);

        LogHelper.LogWarning(_logger, "No user ID found in the token.");
        throw new UnauthorizedAccessException("User is not authenticated.");
    }

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }

    public async Task<UserDto> ConfirmEmailAsync(Guid userId)
    {
        LogHelper.LogInfo(_logger, "Starting email confirmation for user: {UserId}", [userId]);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            LogHelper.LogWarning(_logger, "User with ID {UserId} not found for email confirmation.", [userId]);
            throw new Exception($"User with ID '{userId}' not found.");
        }

        if (user.EmailConfirmed)
        {
            LogHelper.LogWarning(_logger, "Attempt to confirm an already confirmed email for user {UserId}.", [userId]);
            throw new InvalidOperationException("Email already confirmed.");
        }

        user.EmailConfirmed = true;
        await _userRepository.UpdateAsync(user);

        LogHelper.LogInfo(_logger, "Email confirmed for user {UserId}.", [userId]);

        await _messageBrokerPublisher.PublishEmailConfirmedAsync(user);

        LogHelper.LogInfo(_logger, "Email confirmation message published for user {UserId}.", [userId]);
        
        var cacheKey = $"user:{userId}";
        await _cacheService.RemoveAsync(cacheKey);

        return UserMapper.Map(user);
    }
}
