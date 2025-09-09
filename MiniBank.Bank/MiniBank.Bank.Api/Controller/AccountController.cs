using Microsoft.AspNetCore.Mvc;
using MiniBank.Bank.Core.Dto;
using MiniBank.Bank.Core.Interface;

namespace MiniBank.Bank.Api.Controller;

[Route("api/accounts")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetAccountByUserId(Guid userId)
    {
        try
        {
            var account = await accountService.GetAccountByUserIdAsync(userId);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPost("{userId:guid}/deposit")]
    public async Task<IActionResult> DepositByUserId(Guid userId, [FromBody] DepositRequestDto depositRequestDto)
    {
        try
        {
            var account = await accountService.DepositByUserIdAsync(userId, depositRequestDto);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{userId:guid}/withdraw")]
    public async Task<IActionResult> WithdrawByUserId(Guid userId, [FromBody] WithdrawRequestDto withdrawRequestDto)
    {
        try
        {
            var account = await accountService.WithdrawByUserIdAsync(userId, withdrawRequestDto);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}