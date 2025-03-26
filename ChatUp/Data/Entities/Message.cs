namespace ChatUp.Data.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }
    public required int SenderId { get; set; }
    public required int RelationId { get; set; }
    public required string Content { get; set; }
    public required long CreatedAt { get; set; } // Unix timestamp

    // Navigation properties
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; }
    [ForeignKey("RelationId")]
    public virtual Relation Relation { get; set; }
}
