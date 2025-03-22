using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizerAttribute : Attribute, IAsyncActionFilter
{
    private Repository<Session> _sessionRepository;
    private Repository<User> _userRepository;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        // Get repositories
        var sessionRepository = context.HttpContext.RequestServices.GetService<Repository<Session>>();
        var userRepository = context.HttpContext.RequestServices.GetService<Repository<User>>();

        if (_sessionRepository is null || _userRepository is null)
        {
            throw new InvalidOperationException("Filed to get repository");
        }

        _sessionRepository = sessionRepository;
        _userRepository = userRepository;


        // Get token from cookie or header
        string? token = httpContext.Request.Cookies["AuthToken"]
                  ?? httpContext.Request.Headers["AuthToken"].ToString();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from cookie or header
        string? userIdString = httpContext.Request.Cookies["UserId"]
                  ?? httpContext.Request.Headers["UserId"];

        if (string.IsNullOrEmpty(userIdString))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Parse user ID
        if (!int.TryParse(userIdString, out int userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // check token and id in db
        if (!await IsValidTokenAsync(userId, token))
        {
            context.Result = new UnauthorizedResult();
            return;

        }

        // Authorized
        await next();
    }

    private async Task<bool> IsValidTokenAsync(int userId, string token)
    {
        var session = await _sessionRepository.GetAsync(u => u.Token == token);
        return session is not null && session.User.Id == userId;
    }
}