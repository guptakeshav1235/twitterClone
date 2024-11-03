using Microsoft.EntityFrameworkCore;
using twitter.api.Data;
using twitter.api.Models.Domain;

namespace twitter.api.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            return post;
        }

        public async Task<IEnumerable<Post>> GetAllPostAsync()
        {
            return await dbContext.Posts
                        .Include(x => x.User)
                        .Include(x => x.Comments)
                            .ThenInclude(x => x.User)
                        .OrderByDescending(x => x.CreatedAt)
                        .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await dbContext.Posts
                .Include(x=>x.Comments)
                .Include(x=>x.Likes)
                .FirstOrDefaultAsync(x=>x.Id==postId);
        }

        public async Task DeletePostAsync(Post post)
        {
            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync();
        }

        public async Task CommentOnPost(Post post, Comment comment)
        {
            post.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
        }

        public async Task LikeUnlikePostAsync(Post post, Guid userId, bool isLike)
        {
            if (isLike)
            {
                //Like Post
                var userToLike = await dbContext.Users.FindAsync(userId);
                post.Likes.Add(userToLike);
                await dbContext.SaveChangesAsync();

                //Send Notification to the user
                var newNotification = new Notification
                {
                    Type = NotificationType.Like,
                    UserIdFrom = userId,
                    UserIdTo = post.UserId,
                };

                dbContext.Notifications.Add(newNotification);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                //Unlike Post
                var userToUnlike= post.Likes.FirstOrDefault(user => user.Id == userId);
                post.Likes.Remove(userToUnlike);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetFollowingPostAsync(Guid userId)
        {
            var user = await dbContext.Users
                .Include(x => x.Following)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null || !user.Following.Any())
                return new List<Post>();

            var followingIds=user.Following.Select(x=>x.Id).ToList();

            var feedPosts=await dbContext.Posts
                .Where(p=>followingIds.Contains(p.UserId))
                .Include(p=>p.User)
                .Include(p=>p.Comments).ThenInclude(c=>c.User)
                .OrderByDescending(p=>p.CreatedAt)
                .ToListAsync();

            return feedPosts;
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(string username)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            if(user==null)
                return new List<Post>();

            var posts= await dbContext.Posts
                .Where(p => p.UserId==user.Id)
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts;
        }
    }
}
