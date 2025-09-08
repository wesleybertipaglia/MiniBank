using Microsoft.AspNetCore.Mvc;
using MiniBank.Auth.Core.Dto;
using MiniBank.Auth.Core.Interface;

namespace MiniBank.Auth.Api.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequestDto signInRequestDto)
    {
        try
        {
            var response = await authService.SignIn(signInRequestDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                return Unauthorized(ex.Message);
            }

            return BadRequest(ex.Message);
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto signUpRequestDto)
    {
        try
        {
            var response = await authService.SignUp(signUpRequestDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
