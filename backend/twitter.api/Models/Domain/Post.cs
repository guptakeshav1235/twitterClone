using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace twitter.api.Models.Domain
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public string? Text { get; set; }

        public string? Img { get; set; }

        public ICollection<User> Likes { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public DateTime UpdatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
