namespace ChatUp.Data.Entities;

public class Relation
{
    [Key]
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int FriendId { get; set; }
    public int Status { get; set; } // 0: Pending, 1: Accepted, 2: Rejected

    // Navigation properties
    public User Sender { get; set; }
    public User Friend { get; set; }
    public ICollection<Message> Messages { get; set; }
}
