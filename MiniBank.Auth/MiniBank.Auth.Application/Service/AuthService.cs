using Microsoft.Extensions.Configuration;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Helpers;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;

namespace MiniBank.Auth.Application.Service;

public class AuthService(IUserRepository userRepository, IConfiguration configuration)
    : IAuthService
{
    public async Task<AuthResponseDto> SignIn(SignInRequestDto signInRequestDto)
    {
        var user = await userRepository.GetByEmailAsync(signInRequestDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(signInRequestDto.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }
        
        var token = TokenHelper.GenerateJwtToken(configuration, user);

        return AuthMapper.Map
        (
            UserMapper.Map(user),
            TokenMapper.Map(token)
        );
    }
    
    public async Task<AuthResponseDto> SignUp(SignUpRequestDto signUpRequestDto)
    {
        var existingUser = await userRepository.GetByEmailAsync(signUpRequestDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already in use");
        }
        
        var user = UserMapper.Map(signUpRequestDto);
        await userRepository.CreateAsync(user);
        var token = TokenHelper.GenerateJwtToken(configuration, user);
        
        return AuthMapper.Map
        (
            UserMapper.Map(user),
            TokenMapper.Map(token)
        );
    }
}