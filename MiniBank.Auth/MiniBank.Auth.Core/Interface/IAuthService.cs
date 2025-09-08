using MiniBank.Auth.Core.Dto;

namespace MiniBank.Auth.Core.Interface;

public interface IAuthService
{
    Task<AuthResponseDto> SignIn(SignInRequestDto  signInRequestDto);
    Task<AuthResponseDto> SignUp(SignUpRequestDto signUpRequestDto);
}