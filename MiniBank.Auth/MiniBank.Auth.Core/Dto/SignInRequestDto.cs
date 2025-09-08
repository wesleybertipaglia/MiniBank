namespace MiniBank.Auth.Core.Dto;

public record SignInRequestDto
(
    string Email,
    string Password
);