using MiniBank.Auth.Core.Dto;

namespace MiniBank.Auth.Core.Mapper;

public class TokenMapper
{
    public static TokenDto Map(string token)
    {
        return new TokenDto
        (
            token, 
            DateTime.Now.AddHours(1)
        );
    }
}