using System.ComponentModel.DataAnnotations;

namespace MiniBank.Bank.Core.Dto;

public record DepositRequestDto
(
    [Range(0.01, double.MaxValue, ErrorMessage = "Deposit amount must be greater than zero.")]
    decimal Amount
);