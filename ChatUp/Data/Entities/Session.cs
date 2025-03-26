namespace ChatUp.Data.Entities;

public class Session
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required long ExpirationAt { get; set; }

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
