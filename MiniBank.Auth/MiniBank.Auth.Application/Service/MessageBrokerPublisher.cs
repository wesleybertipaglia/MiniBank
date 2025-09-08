using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Core.Mapper;
using MiniBank.Auth.Core.Model;
using Newtonsoft.Json;

namespace MiniBank.Auth.Application.Service;

public class MessageBrokerPublisher : IMessageBrokerPublisher
{
    private readonly IMessageBroker _messageBroker;

    public MessageBrokerPublisher(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task PublishUserCreatedAsync(User user)
    {
        var message = UserMapper.Map(user);
        
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            await _messageBroker.SendMessageAsync("queue_user_created", jsonMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: "  + ex.Message);
        }
    }

    public async Task PublishEmailConfirmedAsync(User user)
    {
        var message = UserMapper.Map(user);

        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            await _messageBroker.SendMessageAsync("queue_email_confirmed", jsonMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: "  + ex.Message);
        }
    }
}
