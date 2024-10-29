using System.ComponentModel.DataAnnotations;

namespace twitter.api.Models.DTO
{
    public class UserLoginDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
