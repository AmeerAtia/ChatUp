using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;
using ChatUp.Services.Authorization;
using System.Security.Cryptography;

namespace ChatUp.Api.Authorization;

[ApiController, Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CookieOptions cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = true, 
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(15)
    };

    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
    {
        if (!await _authService.SignUp(request.Name, request.Email, request.Password))
            return BadRequest("Email already exists");
        else
            return Ok("User registered successfully.");
    }

    /// <summary>
    /// Logs in a user and save a Session then return the Token
    /// </summary>
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
    {
        // Call sign in from authprization service and validate the result
        var result = await _authService.SignIn(request.Email, request.Password);
        if (result is null)
            return Unauthorized("Invalid email or password.");

        // Determine if the client is a web app
        bool isWebClient = HttpContext.Request.Headers["Client-Type"] == "Web";

        // For web clients
        if (isWebClient)
        {
            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);
            Response.Cookies.Append("AuthRefreshToken", result.RefreshToken, cookieOptions);
            return Ok();
        }

        // For other clients
        return Ok(new
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        // Get the RefreshToken from the request cookies or body
        string? refreshToken = HttpContext.Request.Cookies["RefreshToken"]
                            ?? request.Token;

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Refresh token is missing.");

        // Call refresh from authprization service and validate the result
        var result = await _authService.Refresh(refreshToken);

        if (result is null)
            return Unauthorized("Invalid or expired refresh token.");

        // Determine if the client is a web app
        bool isWebClient = HttpContext.Request.Headers["Client-Type"] == "Web";

        // For web clients
        if (isWebClient)
        {
            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);
            Response.Cookies.Append("AuthRefreshToken", result.RefreshToken, cookieOptions);
            return Ok();
        }

        // For other clients
        return Ok(new
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut()
    {
        // Get the AuthToken from cookies or headers
        string? token = HttpContext.Request.Cookies["AuthToken"]
                      ?? HttpContext.Request.Headers["AuthToken"].ToString();

        if (string.IsNullOrEmpty(token))
            return BadRequest("Missing token.");

        // Call signout from authprization service and validate the result
        var result = await _authService.SignOut(token);
        if (!result)
            return BadRequest("Invalid token.");

        // Clear cookies for web clients
        bool isWebClient = HttpContext.Request.Headers["User-Agent"].ToString().Contains("Mozilla");

        if (isWebClient)
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("AuthRefreshToken");
        }

        // Return
        return Ok("Logged out successfully.");
    }
}