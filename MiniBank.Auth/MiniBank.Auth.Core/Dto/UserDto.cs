namespace MiniBank.Auth.Core.Dto;

public record UserDto
(
    Guid Id,
    string Name,
    string Email,
    bool EmailConfirmed
);