using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public BasicUserInfoDto FromUser { get; set; }
        public BasicUserInfoDto ToUser { get;set; }
        public NotificationType Type { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
