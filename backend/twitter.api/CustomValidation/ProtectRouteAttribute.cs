using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using twitter.api.Data;

namespace twitter.api.CustomValidation
{
    public class ProtectRouteAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;

        public ProtectRouteAttribute(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            // Get the token from cookies
            var token = httpContext.Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Unauthorized: No Token Provided");
                return;
            }
            try
            {
                //Validate the token
                var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]);
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userId = principal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("Unauthorized: Invalid Token");
                    return;
                }

                // Retrieve user from database
                var user = await dbContext.Users.FindAsync(new Guid(userId));
                if (user == null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    await httpContext.Response.WriteAsync("User not found");
                    return;
                }

                // Attach user to HttpContext for use in controllers
                httpContext.Items["User"] = user;
                httpContext.Items["UserId"] = userId;
            }
            catch (SecurityTokenException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Unauthorized: Invalid Token");
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsync("Internal Server Error");
            }

        }
    }
}
