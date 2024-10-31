using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace twitter.api.Models.Domain
{
    public enum NotificationType
    {
        Follow,
        Like
    }
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for each user

        [Required]
        public Guid UserIdFrom { get; set; }

        [ForeignKey(nameof(UserIdFrom))]
        public User FromUser { get; set; }

        [Required]
        public Guid UserIdTo { get; set; }

        [ForeignKey(nameof(UserIdTo))]
        public User ToUser { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public bool Read { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
