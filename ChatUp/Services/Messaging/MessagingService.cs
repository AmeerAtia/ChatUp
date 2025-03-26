using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Services.Messaging;

public class MessagingService : IMessagingService
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IRepository<Relation> _relationRepository;

    public MessagingService(IRepository<Message> messageRepository, IRepository<Relation> relationRepository)
    {
        _messageRepository = messageRepository;
        _relationRepository = relationRepository;
    }

    public async Task<bool> SendMessage(User user, int relationId, string content)
    {
        var relation = _relationRepository.Get(relationId);
        if (relation is null)
            return false;

        if (user != relation.Sender &&
            user != relation.Receiver)
            return false;

        var message = new Message()
        {
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SenderId = user.Id,
            RelationId = relationId,
        };

        await _messageRepository.InsertAsync(message);
        await _messageRepository.SaveAsync();
        return true;
    }

    public async Task<bool> EditMessage(User user, int messageId, string newContent)
    {
        var message = await _messageRepository.GetAsync(messageId);
        if (message is null)
            return false;

        if (user != message.Sender)
            return false;

        message.IsEdited = true;

        _messageRepository.Update(message);
        await _messageRepository.SaveAsync();
        return true;
    }

    public async Task<bool> RemoveMessage(User user, int messageId)
    {
        var message = await _messageRepository.GetAsync(messageId);
        if (message is null)
            return false;

        if (user != message.Sender)
            return false;

        _messageRepository.Remove(message);
        await _messageRepository.SaveAsync();
        return true;
    }
}
