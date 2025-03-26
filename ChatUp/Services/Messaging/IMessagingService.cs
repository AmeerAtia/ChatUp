using ChatUp.Data.Entities;

namespace ChatUp.Services.Messaging;

public interface IMessagingService
{
    Task<bool> SendMessage(User user, int relationId, string content);
    Task<bool> EditMessage(User user, int messageId, string newContent);
    Task<bool> RemoveMessage(User user, int messageId);
}
