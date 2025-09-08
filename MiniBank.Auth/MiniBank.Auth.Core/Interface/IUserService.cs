using MiniBank.Auth.Core.Dto;

namespace MiniBank.Auth.Core.Interface;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserDto> ConfirmEmailAsync(Guid userId);
}