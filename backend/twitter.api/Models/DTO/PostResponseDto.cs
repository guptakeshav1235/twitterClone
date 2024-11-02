using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class PostResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string? Text { get; set; }

        public string? Img { get; set; }
        public ICollection<User> Likes { get; set; } = new List<User>();

        public ICollection<CommentDetailDto> Comments { get; set; } = new List<CommentDetailDto>();

        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public DateTime UpdatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
