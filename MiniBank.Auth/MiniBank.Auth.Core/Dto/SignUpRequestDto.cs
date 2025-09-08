namespace MiniBank.Auth.Core.Dto;

public record SignUpRequestDto
(
    string Email,
    string Password,
    string Name
);
