namespace twitter.api.Models.DTO
{
    public class BasicPostInfoDto
    {
        public Guid Id { get; set; }
        public ICollection<BasicUserInfoDto> Likes { get; set; } = new List<BasicUserInfoDto>();
    }
}
