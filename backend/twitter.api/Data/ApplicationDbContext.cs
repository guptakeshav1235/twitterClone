using Microsoft.EntityFrameworkCore;
using twitter.api.Models.Domain;

namespace twitter.api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between User and Post for likes
             modelBuilder.Entity<User>()
                .HasMany(u => u.LikedPost)
                .WithMany(p => p.Likes)
                .UsingEntity(j => j.ToTable("UserPostLikes"));

            // Configure one-to-many relationship for User and Post (Owner)
            modelBuilder.Entity<Post>()
             .HasOne(p => p.User)
             .WithMany()  // No inverse collection in User for authored posts
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Restrict); // Configure cascade behavior as needed

            //Configure one-to-many Relationships
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.FromUser)
                .WithMany()
                .HasForeignKey(n => n.UserIdFrom)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to avoid multiple cascade paths

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ToUser)
                .WithMany()
                .HasForeignKey(n => n.UserIdTo)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascade delete for this relation

            modelBuilder.Entity<Comment>()
                .HasOne(n => n.Post)
                .WithMany(n=>n.Comments)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
