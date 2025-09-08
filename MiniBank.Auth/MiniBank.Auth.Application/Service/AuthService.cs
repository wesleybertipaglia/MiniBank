using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;
using MiniBank.Auth.Core.Model;

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
        
        var token = GenerateJwtToken(user);

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
        var token = GenerateJwtToken(user);
        
        return AuthMapper.Map
        (
            UserMapper.Map(user),
            TokenMapper.Map(token)
        );
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}