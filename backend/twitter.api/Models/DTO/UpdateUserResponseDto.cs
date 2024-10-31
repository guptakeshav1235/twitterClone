using System.ComponentModel.DataAnnotations;
using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class UpdateUserResponseDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? ProfileImg { get; set; }
        public string? CoverImg { get; set; }
        public string? Bio { get; set; }
        public string? Link { get; set; }
    }
}
