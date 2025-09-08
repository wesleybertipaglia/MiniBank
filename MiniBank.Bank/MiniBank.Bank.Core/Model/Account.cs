namespace MiniBank.Bank.Core.Model;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;
    
    public void OpenAccount(Guid userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
            throw new InvalidOperationException("Amount must be greater than zero.");
        
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (!HasSufficientBalance(amount))
            throw new InvalidOperationException("Insufficient balance.");
        
        Balance -= amount;
    }

    private bool HasSufficientBalance(decimal amount)
    {
        return Balance >= amount;
    }
}