using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using twitter.api.CustomValidation;
using twitter.api.Models.DTO;
using twitter.api.Repositories;

namespace twitter.api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [ServiceFilter(typeof(ProtectRouteAttribute))]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IMapper mapper;

        public NotificationController(INotificationRepository notificationRepository, IMapper mapper)
        {
            this.notificationRepository = notificationRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Getnotifications()
        {
            var userId = HttpContext.Items["UserId"] as string;
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var notifications=await notificationRepository.GetNotificationsAsync(Guid.Parse(userId));
            await notificationRepository.UpdateNotificationsAsync(Guid.Parse(userId));
            return Ok(mapper.Map<List<NotificationDto>>(notifications));    
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNotifications()
        {
            var userId = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            await notificationRepository.DeleteNotificationsAsync(Guid.Parse(userId));
            return Ok(new { message = "Notifications deleted successfully" });
        }
    }
}
