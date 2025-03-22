namespace ChatUp.Api.Authorization;

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(8)] public string Password { get; set; }
}

public class RegisterRequest
{
    [Required, MaxLength(20)] public string Name { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(8)] public string Password { get; set; }
}
