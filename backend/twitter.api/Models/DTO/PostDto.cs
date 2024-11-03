using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public BasicUserInfoDto User { get; set; }
        public string? Text { get; set; }
        public ICollection<BasicUserInfoDto> Likes { get; set; } = new List<BasicUserInfoDto>();
        public ICollection<CommentDetailDto> Comments { get; set; } = new List<CommentDetailDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
