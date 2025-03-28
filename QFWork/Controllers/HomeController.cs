using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models;

namespace QFWork.Controllers;

[Authorize]
public class HomeController : Controller
{
    private ApplicationDbContext _context;
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (user == null)
        {
            return Unauthorized(); 
        }
        var userId = Guid.Parse(user);
        var courses = await _context.Courses
                            .Where(c => c.Teachers.Contains(userId) || c.Students.Contains(userId))
                            .AsNoTracking()
                            .ToListAsync();

        return View(courses);
    }
    [HttpGet]
    public IActionResult Join()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Join(string courseCode)
    {
        // Check if the courseCode is a valid Guid
        if (string.IsNullOrEmpty(courseCode) || !Guid.TryParse(courseCode, out Guid courseId))
        {
            ModelState.AddModelError("courseCode", "Invalid course code.");
            return View(); 
        }

        var course = _context.Set<Course>().FirstOrDefault(c => c.PublicKey == courseId);

        // Check if the course exists
        if (course == null)
        {
            ModelState.AddModelError("courseCode", "Course not found.");
            return View(); 
        }

        // Check if the user is authenticated and has a valid NameIdentifier
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }
        // Check the role of the user (Teacher or Student)
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole))
        {
            return Unauthorized("User role is not defined.");
        }

        // Ensure the user is not already enrolled in the course (as the same role)
        if (userRole == "Student" && course.Students.Contains(Guid.Parse(userId)))
        {
            ModelState.AddModelError("courseCode", "You are already enrolled as a Student in this course.");
            return View();
        }

        if (userRole == "Teacher" && course.Teachers.Contains(Guid.Parse(userId)))
        {
            ModelState.AddModelError("courseCode", "You are already enrolled as a Teacher in this course.");
            return View();
        }

        // Add the user to the course based on their role
        if (userRole == "Student")
        {
            course.Students.Add(Guid.Parse(userId));
        }
        else if (userRole == "Teacher")
        {
            course.Teachers.Add(Guid.Parse(userId));
        }
        else
        {
            return Unauthorized("Role is not recognized.");
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "Teacher")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    public IActionResult Create(string courseName)
    {
        var teacherId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (teacherId == null)
        {
            return Unauthorized(); 
        }

        Course course = new Course()
        {
            Name = courseName,
            Teachers = new List<Guid>() { Guid.Parse(teacherId) } 
        };

        _context.Courses.Add(course);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Leave(int courseId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var userGuid = Guid.Parse(userId);

        if (course.Students.Contains(userGuid))
        {
            course.Students.Remove(userGuid);
        }
        else if (course.Teachers.Contains(userGuid))
        {
            course.Teachers.Remove(userGuid);
        }
        else
        {
            return BadRequest("User is not enrolled in this course.");
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
