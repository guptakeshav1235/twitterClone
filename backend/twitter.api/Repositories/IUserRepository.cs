using twitter.api.Models.Domain;
using twitter.api.Models.DTO;

namespace twitter.api.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserWithLikesAsync(Guid userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> ids);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<string> FollowUnfollowUserAsync(Guid curerntUserId, Guid userToModify);
        Task<IEnumerable<User>> GetSuggestedUserAsync(Guid userId);
        Task<User> UpdateUserAsync(User user);
    }
}
