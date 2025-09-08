using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Model;

namespace MiniBank.Auth.Core.Mapper;

public static class UserMapper
{
    public static User Map(SignUpRequestDto signUpRequestDto)
    {
        return new User()
        {
            Name = signUpRequestDto.Name,
            Email = signUpRequestDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(signUpRequestDto.Password),
        };
    }

    public static UserDto Map(User user)
    {
        return new UserDto
        (
            user.Id,
            user.Name,
            user.Email,
            user.EmailConfirmed
        );
    }
}