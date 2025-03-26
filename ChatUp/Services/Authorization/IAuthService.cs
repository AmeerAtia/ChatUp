using ChatUp.Data.Entities;

namespace ChatUp.Services.Authorization;

public interface IAuthService
{
    Task<User?> Validate(string token);
    Task<bool> SignUp(string name, string email, string password);
    Task<AuthServiceTokenResult?> SignIn(string email, string password);
    Task<AuthServiceTokenResult?> Refresh(string refreshToken);
    Task<bool> SignOut(string token);
}

public class AuthServiceTokenResult
{
    public required string Token;
    public required string RefreshToken;
}