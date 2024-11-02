using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class CommentDetailDto
    {
        public Guid Id { get; set; }
        public BasicUserInfoDto User { get; set; }
        public string Text { get; set; }
    }
}
