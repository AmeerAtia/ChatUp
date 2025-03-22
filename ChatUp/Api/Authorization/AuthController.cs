using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;

namespace ChatUp.Api.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IRepository<User> userRepository, IRepository<Session> sessionRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Check if the Email is already registered
        if (await _userRepository.ExistsAsync(u => u.Email == request.Email))
        {
            return BadRequest("Email already exists");
        }

        // Create a new user
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Passhash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        // Save the user to the database
        await _userRepository.InsertAsync(user);
        await _userRepository.SaveAsync();

        return Ok("User registered successfully.");
    }

    /// <summary>
    /// Logs in a user and save a Session then return the Token
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Find the user by email
        var user = await _userRepository.GetAsync(u => u.Email == request.Email);
        if (user is null)
            return Unauthorized("Invalid email or password.");

        // Verify the password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Passhash))
            return Unauthorized("Invalid email or password.");

        // Generate a token and refrech token
        string token = Guid.NewGuid().ToString("N");
        string refreshToken = Guid.NewGuid().ToString("N");

        // Create and save the Session to the database
        var session = new Session
        {
            UserId = user.Id,
            Token = token,
            ExpirationAt = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds(),
            RefreshToken = refreshToken,
        };

        await _sessionRepository.InsertAsync(session);
        await _sessionRepository.SaveAsync();

        // Determine if the client is a web app
        bool isWebClient = HttpContext.Request.Headers["User-Agent"].ToString().Contains("Mozilla");

        // For web clients
        if (isWebClient)
        {
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(20)
            });

            return Ok(user.Id);
        }
        
        // For other clients
        return Ok(new
        {
            Token = token,
            UserId = user.Id
        });
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        // Get the RefreshToken from the request body or cookies
        string? refreshToken = HttpContext.Request.Cookies["RefreshToken"]
                            ?? request.Token;

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        // Get the UserId token from the request body or cookies
        string? userIdString = HttpContext.Request.Cookies["UserId"]
                            ?? request.UserId;

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("User id is missing.");
        }

        // Parse user ID
        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized("User id is invalid.");
        }

        // Find the session by Refresh token and User id
        var session = await _sessionRepository.GetAsync(s =>
            s.RefreshToken == refreshToken &&
            s.UserId == userId
        );

        if (session is null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        // Generate a new access token
        string newToken = Guid.NewGuid().ToString("N");

        // Update the session with the new access token and expiration
        session.Token = newToken;
        session.ExpirationAt = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();

        _sessionRepository.Update(session);
        await _sessionRepository.SaveAsync();

        // Determine if the client is a web app
        bool isWebClient = HttpContext.Request.Headers["User-Agent"].ToString().Contains("Mozilla");

        // For web clients
        if (isWebClient)
        {
            Response.Cookies.Append("AuthToken", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            return Ok();
        }

        // For other clients
        return Ok(new
        {
            Token = newToken
        });
    }
}