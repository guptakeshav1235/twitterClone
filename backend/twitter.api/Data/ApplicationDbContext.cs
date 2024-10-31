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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }


    }
}
