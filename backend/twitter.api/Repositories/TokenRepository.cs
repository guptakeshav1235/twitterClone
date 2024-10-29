using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace twitter.api.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void GenerateTokenAndSetCookie(Guid userId, HttpResponse response)
        {
            //Generate Jwt Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", userId.ToString()) }) /*Add Claims */,
                Expires = DateTime.Now.AddDays(15),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //Set Cookie
            response.Cookies.Append("jwt", tokenString, new CookieOptions
            {
                MaxAge = TimeSpan.FromHours(1),
                HttpOnly = true, //prevent XSS attacks cross - site scripting attacks
                SameSite = SameSiteMode.Strict, // CSRF attacks cross-site request forgery attacks
                Secure = configuration["ASPNETCORE_ENVIRONMENT"] != "Development"
            });
        }
    }
}
