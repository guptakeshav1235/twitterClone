using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class BasicUserInfoDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string? ProfileImg { get; set; }
    }
}
