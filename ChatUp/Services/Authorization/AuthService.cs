using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Services.Authorization;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Session> _sessionRepository;

    public AuthService(IRepository<User> userRepository, IRepository<Session> sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<User?> Validate(string token)
    {
        // Get session
        var session = await _sessionRepository.GetAsync(s =>
            s.Token == token &&
            s.ExpirationAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        );

        // Invalid
        if (session is null)
            return null;

        // Valid
        return session.User;
    }

    public async Task<bool> SignUp(string name, string email, string password)
    {
        // Check if the Email is already registered
        if (await _userRepository.ExistsAsync(u => u.Email == email))
        {
            return false;
        }

        // Create a new user
        var user = new User
        {
            Name = name,
            Email = email,
            Passhash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        // Save the user to the database
        await _userRepository.InsertAsync(user);
        await _userRepository.SaveAsync();

        // Return
        return true;
    }

    public async Task<AuthServiceTokenResult?> SignIn(string email, string password)
    {
        // Find the user by email and Verify the password
        var user = await _userRepository.GetAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Passhash))
            return null;

        // Generate a token and refrech token
        string token = GenerateSecureToken();
        string refreshToken = GenerateSecureToken();

        // Create and save the Session to the database
        var session = new Session
        {
            UserId = user.Id,
            Token = token,
            RefreshToken = refreshToken,
            ExpirationAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };

        await _sessionRepository.InsertAsync(session);
        await _sessionRepository.SaveAsync();

        // Return
        return new AuthServiceTokenResult()
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthServiceTokenResult?> Refresh(string refreshToken)
    {
        // Find the session by Refresh token and User id
        var session = await _sessionRepository.GetAsync(s => s.RefreshToken == refreshToken);

        if (session is null)
            return null;

        // 
        if (session.ExpirationAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            return null;

        // Generate a token and refrech token
        string newToken = GenerateSecureToken();
        string newRefreshToken = GenerateSecureToken();

        // Update the session with the new access token and expiration
        session.Token = newToken;
        session.RefreshToken = newRefreshToken;
        session.ExpirationAt = DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds();

        _sessionRepository.Update(session);
        await _sessionRepository.SaveAsync();

        // Return the token
        return new AuthServiceTokenResult()
        {
            Token = newToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> SignOut(string token)
    {
        // Find the session by token
        var session = await _sessionRepository.GetAsync(s => s.Token == token);

        if (session is null)
            return false;

        // Delete the session from the database
        _sessionRepository.Remove(session);
        await _sessionRepository.SaveAsync();

        // Return
        return true;
    }

    public static string GenerateSecureToken(int length = 64)
    {
        byte[] randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-').Replace('/', '_').Replace("=", "");
    }
}