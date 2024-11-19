namespace twitter.api.Models.DTO
{
    public class SuggestedUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string? ProfileImg { get; set; }
    }
}
