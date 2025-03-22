namespace ChatUp.Data.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int RelationId { get; set; }
    public string Content { get; set; }
    public long CreatedAt { get; set; } // Unix timestamp

    // Navigation properties
    public virtual User Sender { get; set; }
    public virtual Relation Relation { get; set; }
}
