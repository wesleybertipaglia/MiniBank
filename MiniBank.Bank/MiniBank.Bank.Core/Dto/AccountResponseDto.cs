namespace MiniBank.Bank.Core.Dto;

public record AccountResponseDto
(
    Guid Id,
    decimal Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt
);