using twitter.api.Models.Domain;

namespace twitter.api.Repositories
{
    public interface INotificationRepository
    {
        public Task<IEnumerable<Notification>> GetNotificationsAsync(Guid userId);
        public Task UpdateNotificationsAsync(Guid userId);
        public Task DeleteNotificationsAsync(Guid userId);
    }
}
