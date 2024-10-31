using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class UserProfileDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ICollection<User> Followers { get; set; } = new List<User>();
        public ICollection<User> Following { get; set; } = new List<User>();
        public string? ProfileImg { get; set; }
        public string? CoverImg { get; set; }
        public string? Bio { get; set; }
        public string? Link { get; set; }
    }
}
