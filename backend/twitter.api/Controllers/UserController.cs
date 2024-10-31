using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using twitter.api.CustomValidation;
using twitter.api.Models.Domain;
using twitter.api.Models.DTO;
using twitter.api.Repositories;

namespace twitter.api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ICloudinaryRepository cloudinaryRepository;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository, ICloudinaryRepository cloudinaryRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.cloudinaryRepository = cloudinaryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("profile/{username}")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> GetUserProfileAsync([FromRoute] string username)
        {
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            //Map the domain model to dto
            var userDto = mapper.Map<UserProfileDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        [Route("follow/{id:Guid}")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> FollowUnfollowUser([FromRoute] Guid id)
        {
            // Retrieve the current user ID from claims
            var currentuserId = HttpContext.Items["UserId"] as string;

            if (string.IsNullOrEmpty(currentuserId))
                return Unauthorized("User ID not found in token.");

            var resultMessage = await userRepository.FollowUnfollowUserAsync(id, Guid.Parse(currentuserId));

            if (resultMessage.Contains("can't follow") || resultMessage.Contains("User not found"))
            {
                return BadRequest(resultMessage);
            }

            return Ok(resultMessage);
        }

        [HttpGet]
        [Route("suggested")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> GetSuggestedUser()
        {
            // Retrieve the user ID from claims
            var userId = HttpContext.Items["UserId"] as string;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var suggestedUsers = await userRepository.GetSuggestedUserAsync(Guid.Parse(userId));

            //Map Domain to Dto
            var suggestedUsersDto = mapper.Map<IEnumerable<SuggestedUserDto>>(suggestedUsers);
            return Ok(suggestedUsersDto);
        }

        [HttpPost]
        [Route("update")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            // Retrieve the user ID from claims
            var userId = HttpContext.Items["UserId"] as string;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var user = await userRepository.GetUserByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                user.Username = updateUserDto.Username;

            if (!string.IsNullOrEmpty(updateUserDto.FullName))
                user.FullName = updateUserDto.FullName;

            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;

            if (!string.IsNullOrEmpty(updateUserDto.Bio))
                user.Bio = updateUserDto.Bio;

            if (!string.IsNullOrEmpty(updateUserDto.Link))
                user.Link = updateUserDto.Link;

            if ((!string.IsNullOrEmpty(updateUserDto.NewPassword)
                && string.IsNullOrEmpty(updateUserDto.CurrentPassword))
                || (!string.IsNullOrEmpty(updateUserDto.CurrentPassword)
                && string.IsNullOrEmpty(updateUserDto.NewPassword)))
            {
                return BadRequest(new { error = "Both current and new password must be provided" });
            }

            if (!string.IsNullOrEmpty(updateUserDto.CurrentPassword) && !string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                var isMatch = BCrypt.Net.BCrypt.Verify(updateUserDto.CurrentPassword, user.Password);

                if (!isMatch)
                {
                    return BadRequest(new { error = "Current password is incorrect" });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.NewPassword, 10);
            }

            // Handle profile image and cover image updates using cloudinary or another image service
            if (!string.IsNullOrEmpty(updateUserDto.ProfileImg))
            {
                if (!string.IsNullOrEmpty(user.ProfileImg))
                {
                    await cloudinaryRepository.DeleteImageAsync(user.ProfileImg);
                }
                user.ProfileImg = await cloudinaryRepository.UploadImageAsync(updateUserDto.ProfileImg);
            }

            if (!string.IsNullOrEmpty(updateUserDto.CoverImg))
            {
                if (!string.IsNullOrEmpty(user.CoverImg))
                {
                    await cloudinaryRepository.DeleteImageAsync(user.CoverImg);
                }
                user.CoverImg = await cloudinaryRepository.UploadImageAsync(updateUserDto.CoverImg);
            }

            user.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")); // Update timestamp

            var updatedUser = await userRepository.UpdateUserAsync(user);

            //Map Domain model to Dto and return Ok
            return Ok(mapper.Map<UpdateUserResponseDto>(updatedUser));
        }
    }
}
