using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;
using ChatUp.Services.Authorization;

namespace ChatUp.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizerAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _parameterName;

    public AuthorizerAttribute(string parameterName = "user")
    {
        _parameterName = parameterName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var authService = httpContext.RequestServices.GetService<AuthService>();

        // Get token from cookie or header
        string? token = httpContext.Request.Cookies["AuthToken"]
                  ?? httpContext.Request.Headers["AuthToken"].ToString();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Validate Token
        var user = await authService.Validate(token);
        if (user is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Return user to the method
        var parameter =context.ActionDescriptor.Parameters.FirstOrDefault(p =>
            p.Name.Equals(_parameterName, StringComparison.OrdinalIgnoreCase) &&
            p.ParameterType == typeof(User)
        );

        if (parameter is not null)
            context.ActionArguments[parameter.Name] = user;

        // Continue to the Endpoint
        await next();
    }
}