using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class UserProfileDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ICollection<BasicUserInfoDto> Followers { get; set; } = new List<BasicUserInfoDto>();
        public ICollection<BasicUserInfoDto> Following { get; set; } = new List<BasicUserInfoDto>();
        public ICollection<BasicPostInfoDto> LikedPost { get; set; } = new List<BasicPostInfoDto>();
        public string? ProfileImg { get; set; }
        public string? CoverImg { get; set; }
        public string? Bio { get; set; }
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    }
}
