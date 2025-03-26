using ChatUp.Api.Authorization;
using ChatUp.Data.Entities;
using ChatUp.Services.Relations;

namespace ChatUp.Api.Relations;

[ApiController, Route("api/[controller]")]
public class RelationsController : ControllerBase
{
    private readonly IRelationsService _relationsService;

    public RelationsController(IRelationsService relationsService)
    {
        _relationsService = relationsService;
    }

    /// <summary>
    /// Send relationship request
    /// </summary>
    [HttpPost("request/{receiverId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorizer("user")]
    public async Task<IActionResult> RequestRelation(User user, int receiverId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.RequestRelation(user,receiverId);

        return result ? Ok() : BadRequest("Invalid relationship request");
    }

    /// <summary>
    /// Accept pending relationship request
    /// </summary>
    [HttpPut("accept/{senderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> AcceptRelation(User user, int senderId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.AcceptRelation(user, senderId);

        return result ? Ok() : NotFound("Pending request not found");
    }

    /// <summary>
    /// Remove existing relationship
    /// </summary>
    [HttpDelete("remove/{friendId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> RemoveRelation(User user, int friendId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.RemoveRelation(user, friendId);

        return result ? Ok() : NotFound("Relationship not found");
    }

    /// <summary>
    /// Block a user
    /// </summary>
    [HttpPost("block/{blockedId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorizer("user")]
    public async Task<IActionResult> BlockRelation(User user, int userId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.BlockRelation(user, userId);

        return result ? Ok() : BadRequest("Block operation failed");
    }

    /// <summary>
    /// Unblock a user
    /// </summary>
    [HttpDelete("unblock/{blockedId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> UnblockRelation(User user, int blockedId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.UnblockRelation(user, blockedId);

        return result ? Ok() : NotFound("Block relationship not found");
    }

    /// <summary>
    /// Get user's friends
    /// </summary>
    [HttpDelete("friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> GetFriends(User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.GetFriends(user);

        return Ok(result);
    }

    /// <summary>
    /// Get all users that the user blocked
    /// </summary>
    [HttpDelete("userblocked")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> GetUserBlocked(User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.GetUserBlocked(user);

        return Ok(result);
    }

    /// <summary>
    /// Get all users that blocked the user
    /// </summary>
    [HttpDelete("blockedusers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorizer("user")]
    public async Task<IActionResult> GetBlockedUsers(User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _relationsService.GetBlockedUsers(user);

        return Ok(result);
    }
}