namespace ChatUp.Data.Entities;

public class Session
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public long ExpirationAt { get; set; }

    // Navigation property
    public virtual User User { get; set; }
}
