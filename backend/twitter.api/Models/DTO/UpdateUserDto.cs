using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using twitter.api.Models.Domain;

namespace twitter.api.Models.DTO
{
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MinLength(6)]
        public string? CurrentPassword { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(280)]
        public string? Bio { get; set; }

        [Url]
        public string? Link { get; set; }

        public string? ProfileImg { get; set; }
        public string? CoverImg { get; set; }
    }
}
