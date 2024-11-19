using twitter.api.Models.Domain;

namespace twitter.api.Repositories
{
    public interface IPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        Task<IEnumerable<Post>> GetAllPostAsync();
        Task<Post> GetPostByIdAsync(Guid postId);
        Task DeletePostAsync(Post post);
        Task CommentOnPost(Post post, Comment comment);
        Task LikeUnlikePostAsync(Post post, Guid userId, bool isLike);
        Task<IEnumerable<Post>> GetFollowingPostAsync(Guid userId);
        Task<IEnumerable<Post>> GetUserPostsAsync(string username);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<Post>> GetPostsByIdsAsync(IEnumerable<Guid> postIds);
    }
}
