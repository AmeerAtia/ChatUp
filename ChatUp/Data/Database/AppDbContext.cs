namespace ChatUp.Data.Database;

using ChatUp.Data.Entities;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Relation> Relations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User entity
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Relation entity
        modelBuilder.Entity<Relation>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Relation>()
            .HasOne(r => r.Sender)
            .WithMany(u => u.SentRelations)
            .HasForeignKey(r => r.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Relation>()
            .HasOne(r => r.Receiver)
            .WithMany(u => u.ReceivedRelations)
            .HasForeignKey(r => r.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Relation>()
            .Property(r => r.Status)
            .HasConversion<int>();

        // Message entity
        modelBuilder.Entity<Message>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Relation)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RelationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Session entity
        modelBuilder.Entity<Session>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
