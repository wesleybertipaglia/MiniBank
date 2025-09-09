namespace MiniBank.Bank.Core.Dto;

public record AccountResponseDto
(
    Guid Id,
    Guid UserId,
    decimal Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt
);