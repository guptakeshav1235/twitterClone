using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Link { get; set; }
        public string? ProfileImg { get; set; }
        public string? CoverImg { get; set; }
    }
}
