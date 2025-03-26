using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Services.Relations;

public class RelationsService : IRelationsService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<Relation> _relationsRepository;

    public RelationsService(IRepository<User> userRepository, IRepository<Session> sessionRepository, IRepository<Relation> relationsRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _relationsRepository = relationsRepository;
    }

    public async Task<bool> RequestRelation(int userId, int friendId)
    {
        if (userId == friendId) return false;

        // Check for existing relationships
        var existingRelation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == userId && r.ReceiverId == friendId) ||
            (r.SenderId == friendId && r.ReceiverId == userId)
        );

        if (existingRelation is not null)
            return false;

        var relation = new Relation()
        {
            SenderId = userId,
            ReceiverId = friendId,
            Status = RelationStatus.Pending
        };

        await _relationsRepository.InsertAsync(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<bool> AcceptRelation(int userId, int friendId)
    {
        if (userId == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == friendId && r.ReceiverId == userId) ||
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

    public async Task<bool> RemoveRelation(int userId, int friendId)
    {
        if (userId == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == userId && r.ReceiverId == friendId) ||
            (r.Status == RelationStatus.Accepted)
        );

        if (relation is null) return false;

        _relationsRepository.Remove(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<bool> BlockRelation(int userId, int friendId)
    {
        if (userId == friendId) return false;

        // Check for existing relationships
        var existingRelation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == userId && r.ReceiverId == friendId) ||
            (r.SenderId == friendId && r.ReceiverId == userId)
        );

        if (existingRelation is not null)
        {
            existingRelation.Status = RelationStatus.Blocked;
            existingRelation.SenderId = userId;
            existingRelation.ReceiverId = friendId;
            _relationsRepository.Update(existingRelation);
        }
        else
        {
            var newRelation = new Relation()
            {
                SenderId = userId,
                ReceiverId = friendId,
                Status = RelationStatus.Blocked
            };
            await _relationsRepository.InsertAsync(newRelation);
        }

        // Finish
        await _relationsRepository.SaveAsync();
        return true;
    }

    public async Task<bool> UnblockRelation(int userId, int friendId)
    {
        if (userId == friendId) return false;

        // Check for existing relationships
        var relation = await _relationsRepository.GetAsync(r =>
            (r.SenderId == userId && r.ReceiverId == friendId) ||
            (r.Status == RelationStatus.Blocked)
        );

        if (relation is null) return false;

        _relationsRepository.Remove(relation);
        await _relationsRepository.SaveAsync();

        // Return
        return true;
    }
}