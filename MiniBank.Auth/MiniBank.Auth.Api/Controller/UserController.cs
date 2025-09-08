using Microsoft.AspNetCore.Mvc;
using MiniBank.Auth.Core.Interface;

namespace MiniBank.Auth.Api.Controller;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
        
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