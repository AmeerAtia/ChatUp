using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Services.Relations;

public class RelationsService : IRelationsService
{
    private readonly IRepository<Relation> _relationsRepository;

    public RelationsService(IRepository<Relation> relationsRepository)
    {
        _relationsRepository = relationsRepository;
    }

    public async Task<bool> RequestRelation(User user, int friendId)
    {
        if (user.Id == friendId) return false;

        // Check for existing relationships
        var existingRelation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == user.Id && r.ReceiverId == friendId) ||
            (r.SenderId == friendId && r.ReceiverId == user.Id)
        );

        if (existingRelation is not null)
            return false;

        var relation = new Relation()
        {
            SenderId = user.Id,
            ReceiverId = friendId,
            Status = RelationStatus.Pending
        };

        await _relationsRepository.InsertAsync(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<bool> AcceptRelation(User user, int friendId)
    {
        if (user.Id == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == friendId && r.ReceiverId == user.Id) ||
            (r.Status == RelationStatus.Pending)
        );

        if (relation is null)
            return false;

        relation.Status = RelationStatus.Accepted;

        _relationsRepository.Update(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<bool> RemoveRelation(User user, int friendId)
    {
        if (user.Id == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == user.Id && r.ReceiverId == friendId) ||
            (r.Status == RelationStatus.Accepted)
        );

        if (relation is null) return false;

        _relationsRepository.Remove(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<bool> BlockRelation(User user, int friendId)
    {
        if (user.Id == friendId) return false;

        // Check for existing relationships
        var existingRelation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == user.Id && r.ReceiverId == friendId) ||
            (r.SenderId == friendId && r.ReceiverId == user.Id)
        );

        if (existingRelation is not null &&
            existingRelation.Status is not RelationStatus.Blocked)
        {
            existingRelation.Status = RelationStatus.Blocked;
            existingRelation.SenderId = user.Id;
            existingRelation.ReceiverId = friendId;
            _relationsRepository.Update(existingRelation);
        }
        else
        {
            var newRelation = new Relation()
            {
                SenderId = user.Id,
                ReceiverId = friendId,
                Status = RelationStatus.Blocked
            };
            await _relationsRepository.InsertAsync(newRelation);
        }

        // Finish
        await _relationsRepository.SaveAsync();
        return true;
    }

    public async Task<bool> UnblockRelation(User user, int friendId)
    {
        if (user.Id == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == user.Id && r.ReceiverId == friendId) ||
            (r.Status == RelationStatus.Blocked)
        );

        if (relation is null) return false;

        _relationsRepository.Remove(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<IEnumerable<Relation>> GetFriends(User user)
    {
        // Get all related friends relations
        var friends = await _relationsRepository.GetListAsync(f =>
            (f.SenderId == user.Id || f.ReceiverId == user.Id) &&
            f.Status == RelationStatus.Accepted
        );

        // Return
        return friends;
    }

    public async Task<IEnumerable<Relation>> GetUserBlocked(User user)
    {
        // Get all related relations that the user blocked
        var blocked = await _relationsRepository.GetListAsync(f =>
            f.SenderId == user.Id &&
            f.Status == RelationStatus.Blocked
        );

        // Return
        return blocked;
    }

    public async Task<IEnumerable<Relation>> GetBlockedUsers(User user)
    {
        // Get all related relations that blocked the user
        var blocked = await _relationsRepository.GetListAsync(f =>
            f.Receiver.Id == user.Id &&
            f.Status == RelationStatus.Blocked
        );

        // Return
        return blocked;
    }
}