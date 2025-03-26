using System.ComponentModel.DataAnnotations.Schema;

namespace ChatUp.Data.Entities;

public class Relation
{
    [Key]
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public RelationStatus Status { get; set; } // 0: Pending, 1: Accepted, 2: Blocked

    // Navigation properties
    [ForeignKey("SenderId")]
    public User Sender { get; set; }
    [ForeignKey("FriendId")]
    public User Receiver { get; set; }
    public ICollection<Message> Messages { get; set; }
}

public enum RelationStatus
{
    Pending,
    Accepted,
    Blocked
}
