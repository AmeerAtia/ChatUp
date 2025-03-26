namespace ChatUp.Services.Relations;

public interface IRelationsService
{
    Task<bool> RequestRelation(int userId, int friendId);
    Task<bool> AcceptRelation(int userId, int friendId);
    Task<bool> RemoveRelation(int userId, int friendId);
    Task<bool> BlockRelation(int userId, int friendId);
    Task<bool> UnblockRelation(int userId, int friendId);
}