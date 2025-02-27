﻿using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using twitter.api.CustomValidation;
using twitter.api.Models.Domain;
using twitter.api.Models.DTO;
using twitter.api.Repositories;

namespace twitter.api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenRepository tokenRepository;
        private readonly IMapper mapper;

        public AuthController(IUserRepository userRepository, ITokenRepository tokenRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.tokenRepository = tokenRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> signup([FromBody] UserRegisterDto userRegisterDto)
        {
            //ValidateEmail
            if(!isValidEmail(userRegisterDto.Email))
            {
                return BadRequest(new { error = "Invalid Email Format" });
            }

            // Check if username or email already exists
            var existingUser = await userRepository.GetUserByUsernameAsync(userRegisterDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Username is already taken" });
            }

            var existingEmail = await userRepository.GetUserByEmailAsync(userRegisterDto.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { error = "Email is already taken" });
            }

            if (userRegisterDto.Password.Length < 6)
            {
                return BadRequest(new { error = "Password must be at least 6 characters long" });
            }

            //Hash Paswword
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password, 10);

            //Map Dto To Domain Model and then create user
            var userDomainModel = mapper.Map<User>(userRegisterDto);
            userDomainModel.Password = hashedPassword;

            var newUser = await userRepository.CreateUserAsync(userDomainModel);

            if (newUser != null)
            {
                //Generate Token and Set Cookie
                tokenRepository.GenerateTokenAndSetCookie(newUser.Id, Response);

                //Map Domain Model to Dto
                var userDto = mapper.Map<UserResponseDto>(newUser);
                return Ok(userDto);
            }
            else
            {
                return BadRequest(new { error = "Invalid user data" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login([FromBody] UserLoginDto userLoginDto)
        {
            var user = await userRepository.GetUserByUsernameAsync(userLoginDto.Username);
            if(user==null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            {
                return BadRequest(new { error = "Invalid username or password" });
            }

            // Generate Token and Set Cookie
            tokenRepository.GenerateTokenAndSetCookie(user.Id, Response);

            //Map Domain Model to Dto
            var userDto = mapper.Map<UserResponseDto>(user);

            return Ok(userDto);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1), // Set to past date
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/" // Ensure this matches the original cookie path
            });
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet]
        [Route("me")]
        [ServiceFilter(typeof(ProtectRouteAttribute))]
        public async Task<IActionResult> GetMe()
        {
            //var user = HttpContext.Items["User"] as User;
            var userId = Guid.Parse(HttpContext.Items["UserId"] as string);
            var user = await userRepository.GetUserWithLikesAsync(userId);
            if (user == null)
            {
                return Unauthorized("Unauthorized: Invalid or expired token");
            }

            return Ok(mapper.Map<UserResponseDto>(user));
        }

        private bool isValidEmail(string email)
        {
            return new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$").IsMatch(email);
        }
    }
}
