using ChatUp.Data.Entities;

namespace ChatUp.Services.Relations;

public interface IRelationsService
{
    Task<bool> RequestRelation(User user, int friendId);
    Task<bool> AcceptRelation(User user, int friendId);
    Task<bool> RemoveRelation(User user, int friendId);
    Task<bool> BlockRelation(User user, int friendId);
    Task<bool> UnblockRelation(User user, int friendId);

    Task<IEnumerable<Relation>> GetFriends(User user);
    Task<IEnumerable<Relation>> GetUserBlocked(User user);
    Task<IEnumerable<Relation>> GetBlockedUsers(User user);
}