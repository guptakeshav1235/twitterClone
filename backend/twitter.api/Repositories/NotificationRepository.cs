using Microsoft.EntityFrameworkCore;
using twitter.api.Data;
using twitter.api.Models.Domain;

namespace twitter.api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public NotificationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync(Guid userId)
        {
            return await dbContext.Notifications
                            .Where(n => n.UserIdTo == userId)
                            .Include(n => n.FromUser)
                            .ToListAsync();
        }

        public async Task UpdateNotificationsAsync(Guid userId)
        {
            var notifications = await dbContext.Notifications
                                .Where(n => n.UserIdTo == userId&&!n.Read)
                                .ToListAsync();

            foreach(var notification in notifications)
            {
                notification.Read = true;
                notification.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc
                                            (DateTime.UtcNow, TimeZoneInfo.
                                                FindSystemTimeZoneById("India Standard Time"));
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteNotificationsAsync(Guid userId)
        {
            var notifications = await dbContext.Notifications
                                .Where(n => n.UserIdTo == userId)
                                .ToListAsync();

            dbContext.RemoveRange(notifications);
            await dbContext.SaveChangesAsync();
        }
    }
}
