namespace MiniBank.Auth.Core.Dto;

public record AuthResponseDto
(
    UserDto User,
    TokenDto Token
);