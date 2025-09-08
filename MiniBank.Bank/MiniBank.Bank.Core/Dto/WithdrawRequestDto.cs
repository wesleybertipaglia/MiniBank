using System.ComponentModel.DataAnnotations;

namespace MiniBank.Bank.Core.Dto;

public record WithdrawRequestDto
(
    [Range(0.01, double.MaxValue, ErrorMessage = "Withdrawal amount must be greater than zero.")]
    decimal Amount
);