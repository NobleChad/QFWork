using Microsoft.AspNetCore.Mvc;
using QFWork.Atributes;
using QFWork.Data;

namespace QFWork.Controllers
{
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
            return View();
        }
        [HttpGet]
        public IActionResult CreateTask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTask(string name)
        {
            return View();
        }
        public IActionResult Users(int courseId)
        {
            var users = _context.Users.ToList(); 
            return View(users);
        }
    }
}
