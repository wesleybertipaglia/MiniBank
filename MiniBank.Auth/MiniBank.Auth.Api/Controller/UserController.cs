using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Auth.Core.Interface;

namespace MiniBank.Auth.Api.Controller;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var user = await userService.GetCurrentUserAsync();
            return Ok(user);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("confirm-email/{userId:guid}")]
    public async Task<IActionResult> ConfirmEmail([FromRoute] Guid userId)
    {
        try
        {
            var user = await userService.ConfirmEmailAsync(userId);
            return Ok(new { message = "Email confirmado com sucesso!", user });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}