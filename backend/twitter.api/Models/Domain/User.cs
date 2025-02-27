﻿using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace twitter.api.Models.Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for each user

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<User> Followers { get; set; } = new List<User>();
        public ICollection<User> Following { get; set; } = new List<User>();

        public string? ProfileImg { get; set; } 
        public string? CoverImg { get; set; }

        [MaxLength(280)]
        public string? Bio { get; set; }

        public string? Link { get; set; }

        public ICollection<Post> LikedPost { get; set; } = new List<Post>();

        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public DateTime UpdatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

    }
}
