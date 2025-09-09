using Microsoft.AspNetCore.Mvc;
using MiniBank.Mailer.Core.Dto;
using MiniBank.Mailer.Core.Interface;

namespace MiniBank.Mailer.Api.Controller;

[ApiController]
[Route("api/emails")]
public class EmailController(IEmailService emailService) : ControllerBase
{
    [HttpPost]
    [Route("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto emailRequestDto)
    {
        if (string.IsNullOrEmpty(emailRequestDto.To) || string.IsNullOrEmpty(emailRequestDto.Subject) || string.IsNullOrEmpty(emailRequestDto.Body))
        {
            return BadRequest("Something went wrong: one or more fields were required.");
        }
        
        await emailService.SendEmailAsync(emailRequestDto.To, emailRequestDto.Subject, emailRequestDto.Body);
        
        return Ok(new { message = "E-mail successfully sent" });
    }
}