using MiniBank.Auth.Core.Dto;

namespace MiniBank.Auth.Core.Mapper;

public static class AuthMapper
{
    public static AuthResponseDto Map(UserDto userDto, TokenDto tokenDto)
    {
        return new AuthResponseDto(userDto, tokenDto);
    }
}