using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using twitter.api.CustomValidation;
using twitter.api.Models.Domain;
using twitter.api.Models.DTO;
using twitter.api.Repositories;

namespace twitter.api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    [ServiceFilter(typeof(ProtectRouteAttribute))]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private readonly ICloudinaryRepository cloudinaryRepository;
        private readonly IMapper mapper;

        public PostController(IPostRepository postRepository, ICloudinaryRepository cloudinaryRepository, IMapper mapper)
        {
            this.postRepository = postRepository;
            this.cloudinaryRepository = cloudinaryRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        [Route("create")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = HttpContext.Items["UserId"] as string;
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var post = mapper.Map<Post>(createPostDto);
            post.UserId = Guid.Parse(userId);

            if (string.IsNullOrEmpty(createPostDto.Text) && string.IsNullOrEmpty(createPostDto.Img))
            {
                return BadRequest(new { error = "Post musthave text or image" });
            }

            if (!string.IsNullOrEmpty(createPostDto.Img))
            {
                post.Img = await cloudinaryRepository.UploadImageAsync(createPostDto.Img);
            }

            var createdPost = await postRepository.CreatePostAsync(post);
            return Ok(mapper.Map<PostResponseDto>(createdPost));
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllPost()
        {
            var posts = await postRepository.GetAllPostAsync();
            if (posts.Count() == 0)
            {
                return Ok(new List<PostDto>());
            }
            return Ok(mapper.Map<List<PostDto>>(posts));

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var post = await postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            var userId = HttpContext.Items["UserId"] as string;
            if (post.UserId != Guid.Parse(userId))
            {
                return Unauthorized(new { error = "You are not authorized to delete this post" });
            }

            if(!string.IsNullOrEmpty(post.Img))
            {
                await cloudinaryRepository.DeleteImageAsync(post.Img);
            }

            await postRepository.DeletePostAsync(post);

            return Ok("Post deleted successfully");
        }

        [HttpPost]
        [Route("comment/{id:Guid}")]
        public async Task<IActionResult> CommentOnPost([FromRoute] Guid id, [FromBody] CommentDto commentDto)
        {
            if (string.IsNullOrEmpty(commentDto.Text))
            {
                return BadRequest(new { error = "Text field is required" });
            }

            var post = await postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return BadRequest(new { error = "Post not found" });
            }

            var comment = new Comment
            {
                UserId = Guid.Parse(HttpContext.Items["UserId"] as string),
                Text = commentDto.Text,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
            };

            await postRepository.CommentOnPost(post, comment);
            return Ok(mapper.Map<PostResponseDto>(post));
        }

        [HttpPost]
        [Route("like/{id:Guid}")]
        public async Task<IActionResult> LikeUnlikePost([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["UserId"] as string);

            var post = await postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return BadRequest(new { error = "Post not found" });
            }

            var userLikedPost = post.Likes.Select(user => userId).Contains(userId);

            if (userLikedPost)
            {
                //Unlike Post
                await postRepository.LikeUnlikePostAsync(post, userId, isLike: false);
                return Ok(new { message = "Post unliked successfully" });
            }
            else
            {
                //Like Post
                await postRepository.LikeUnlikePostAsync(post, userId, isLike: true);
                return Ok(new { message = "Post liked successfully" });
            }
        }

        [HttpGet]
        [Route("following")]
        public async Task<IActionResult> GetFollowingPost()
        {
            var userId = HttpContext.Items["UserId"]as string;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unauthorized: Invalid or expired token");
            }
            
            var feedPosts=await postRepository.GetFollowingPostAsync(Guid.Parse(userId));
            return Ok(mapper.Map<List<PostDto>>(feedPosts));

        }

        [HttpGet]
        [Route("user/{username}")]
        public async Task<IActionResult> GetUserPosts([FromRoute] string username)
        {
            var posts=await postRepository.GetUserPostsAsync(username);
            if (!posts.Any())
            {
                return NotFound(new { error = "User not found or no posts available" });
            }

            return Ok(mapper.Map<List<PostDto>>(posts));
        }

    }
}
