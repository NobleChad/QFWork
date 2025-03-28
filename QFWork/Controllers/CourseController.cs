using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QFWork.Atributes;
using QFWork.Data;
using QFWork.Models;

namespace QFWork.Controllers
{
    [Authorize]
    [UserInCourse]
    public class CourseController : Controller
    {
        private ApplicationDbContext _context;
        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Details(int id)
        {
            return View(id);
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult CreateTask()
        {
            return View();
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult CreateTask(string name)
        {
            return View();
        }
        public IActionResult Users(int id)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(user);

            var course = _context.Set<Course>().FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            if (!course.Students.Contains(userId) && !course.Teachers.Contains(userId))
            {
                return Unauthorized();
            }

            var userGuids = course.Students.Concat(course.Teachers).ToList();
            var users = _context.Users
                        .AsEnumerable() 
                        .Where(u => userGuids.Contains(Guid.Parse(u.Id)))
                        .ToList();
            return View(users);
        }
    }
}
