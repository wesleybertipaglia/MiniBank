using Microsoft.Extensions.Logging;
using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Helper;
using MiniBank.Bank.Core.Interface;
using MiniBank.Bank.Core.Mapper;
using MiniBank.Bank.Core.Model;

namespace MiniBank.Bank.Application.Service;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMessageBrokerPublisher _messageBrokerPublisher;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository accountRepository,
        IMessageBrokerPublisher messageBrokerPublisher,
        ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _messageBrokerPublisher = messageBrokerPublisher;
        _logger = logger;
    }

    public async Task<AccountResponseDto> OpenAccountByUserIdAsync(UserDto user)
    {
        var existingAccount = await _accountRepository.GetByUserIdAsync(user.Id);
        if (existingAccount != null)
        {
            LogHelper.LogWarning(_logger, $"Account already exists for user {user.Id}");
            throw new InvalidOperationException("Account already exists for this user.");
        }

        var account = new Account();
        account.OpenAccount(user.Id);
        await _accountRepository.CreateAsync(account);
        await _messageBrokerPublisher.PublishAccountCreatedAsync(user);

        LogHelper.LogInfo(_logger, $"Account successfully created for user {user.Id}");

        return AccountMapper.Map(account);
    }

    public async Task<AccountResponseDto> GetAccountByUserIdAsync(Guid userId)
    {
        var account = await _accountRepository.GetByUserIdAsync(userId);

        if (account == null)
        {
            LogHelper.LogWarning(_logger, $"Account not found for user {userId}");
            throw new InvalidOperationException("Account not found for this user.");
        }

        LogHelper.LogInfo(_logger, $"Retrieved account for user {userId}");
        return AccountMapper.Map(account);
    }

    public async Task<AccountResponseDto> DepositByUserIdAsync(Guid userId, DepositRequestDto depositRequestDto)
    {
        var account = await _accountRepository.GetByUserIdAsync(userId);
        
        if (account == null)
        {
            LogHelper.LogWarning(_logger, $"Account not found for deposit by user {userId}");
            throw new InvalidOperationException("Account not found for this user.");
        }

        account.Deposit(depositRequestDto.Amount);
        await _accountRepository.UpdateAsync(account);

        LogHelper.LogInfo(_logger, $"Deposited {depositRequestDto.Amount} to account of user {userId}");

        return AccountMapper.Map(account);
    }

    public async Task<AccountResponseDto> WithdrawByUserIdAsync(Guid userId, WithdrawRequestDto withdrawRequestDto)
    {
        var account = await _accountRepository.GetByUserIdAsync(userId);
        
        if (account == null)
        {
            LogHelper.LogWarning(_logger, $"Account not found for withdrawal by user {userId}");
            throw new InvalidOperationException("Account not found for this user.");
        }

        account.Withdraw(withdrawRequestDto.Amount);
        await _accountRepository.UpdateAsync(account);

        LogHelper.LogInfo(_logger, $"Withdrew {withdrawRequestDto.Amount} from account of user {userId}");

        return AccountMapper.Map(account);
    }
}