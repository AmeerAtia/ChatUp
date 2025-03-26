namespace ChatUp.Data.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Passhash { get; set; }

    // Navigation properties
    public ICollection<Relation> SentRelations { get; set; } = new List<Relation>();
    public ICollection<Relation> ReceivedRelations { get; set; } = new List<Relation>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}