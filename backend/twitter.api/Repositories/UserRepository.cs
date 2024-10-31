using Microsoft.EntityFrameworkCore;
using twitter.api.Data;
using twitter.api.Models.Domain;
using twitter.api.Models.DTO;

namespace twitter.api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await dbContext.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User> GetUserByUsernameFollowerFollowingAsync(string username)
        {
            return await dbContext.Users
                .Include(u => u.Followers) // Include followers
                .Include(u => u.Following) // Include following
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> ids)
        {
            return await dbContext.Users
            .Include(u => u.Followers)
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
        }

        public async Task<string> FollowUnfollowUserAsync(Guid userIdToModify, Guid currentUserId)
        {
            var userToModify = await dbContext.Users.Include(u => u.Followers).FirstOrDefaultAsync(u => u.Id == userIdToModify);
            var currentUser = await dbContext.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (userIdToModify == currentUserId)
            {
                return "You can't follow/unfollow yourself";
            }

            if (userToModify == null || currentUser == null)
            {
                return "User not found";
            }

            // Check if the current user is already following the user to modify
            var isFollowing = currentUser.Following.Any(u => u.Id == userToModify.Id);

            if (isFollowing)
            {
                // Unfollow the user
                currentUser.Following.Remove(userToModify);
                userToModify.Followers.Remove(currentUser);
                await dbContext.SaveChangesAsync();

                return "User unfollowed successfully";
            }
            else
            {
                // Follow the user
                if (!currentUser.Following.Contains(userToModify))
                {
                    currentUser.Following.Add(userToModify);
                    userToModify.Followers.Add(currentUser);
                }
                await dbContext.SaveChangesAsync();

                //Send Notification to the user
                var newNotification = new Notification
                {
                    Type = NotificationType.Follow,
                    UserIdFrom = currentUserId,
                    UserIdTo = userIdToModify,
                };

                dbContext.Notifications.Add(newNotification);
                await dbContext.SaveChangesAsync();

                return "User followed successfully";
            }
        }

        public async Task<IEnumerable<User>> GetSuggestedUserAsync(Guid userId)
        {
            var usersFollowedByme = (await dbContext.Users
                                        .Include(u => u.Following)
                                        .FirstOrDefaultAsync(u => u.Id == userId))
                                        ?.Following ?? Enumerable.Empty<User>();

            var followedUserIds = usersFollowedByme.Select(u => u.Id).ToHashSet();

            var randomUsers = await dbContext.Users
                                    .Where(u => u.Id != userId)
                                    .OrderBy(u => Guid.NewGuid()) //Random ordering
                                    .Take(10)
                                    .ToListAsync();

            var filteredUsers = randomUsers.Where(user => !followedUserIds.Contains(user.Id));
            var suggestedUsers = filteredUsers.Take(4);

            return suggestedUsers;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
    }
}
