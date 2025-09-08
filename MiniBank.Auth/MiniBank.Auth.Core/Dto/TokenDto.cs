namespace MiniBank.Auth.Core.Dto;

public record TokenDto
(
    string Content,
    DateTime Expires
);