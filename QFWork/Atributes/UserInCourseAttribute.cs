using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using QFWork.Data;
using System.Security.Claims;
using Azure.Core;

namespace QFWork.Atributes
{
    public class UserInCourseAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Identity" });
                return;
            }

            var courseId = context.HttpContext.Request.Cookies["CourseId"] ?? context.RouteData.Values["id"]?.ToString();
            if (courseId == null || !int.TryParse(courseId, out var parsedCourseId))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Identity" });
                return;
            }

            var _context = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            var isUserInCourse = _context.Courses
                .Any(e => (e.Teachers.Contains(userId) || e.Students.Contains(userId)) && e.Id == parsedCourseId);

            if (!isUserInCourse)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Identity" });
            }
        }
    }

}
