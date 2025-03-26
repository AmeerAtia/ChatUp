namespace ChatUp.Api.Authorization;

public class LoginRequest
{
    [Required, EmailAddress] public required string Email { get; set; }
    [Required, MinLength(8)] public required string Password { get; set; }
}

public class RegisterRequest
{
    [Required, MaxLength(20)] public required string Name { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
    [Required, MinLength(8)] public required string Password { get; set; }
}

public class RefreshRequest
{
    public required string Token { get; set; }
}
