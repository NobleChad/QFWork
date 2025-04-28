using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QFWork.Models;
using QFWork.Models.Interfaces;
using System.Security.Claims;

namespace QFWork.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ICourseRepository _courseRepository;

    public HomeController(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var courses = await _courseRepository.GetUserCoursesAsync(Guid.Parse(userId));
        return View(courses);
    }

    [HttpGet]
    public IActionResult Join() => View();

    [HttpPost]
    public async Task<IActionResult> Join(string courseCode)
    {
        if (string.IsNullOrEmpty(courseCode) || !Guid.TryParse(courseCode, out Guid courseId))
        {
            ModelState.AddModelError("courseCode", "Invalid course code.");
            return View();
        }

        var course = await _courseRepository.GetCourseByPublicKeyAsync(courseId);
        if (course == null)
        {
            ModelState.AddModelError("courseCode", "Course not found.");
            return View();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            return Unauthorized();

        var userGuid = Guid.Parse(userId);

        if ((userRole == "Student" && course.Students.Contains(userGuid)) ||
            (userRole == "Teacher" && course.Teachers.Contains(userGuid)))
        {
            ModelState.AddModelError("courseCode", "Already enrolled.");
            return View();
        }

        if (userRole == "Student") course.Students.Add(userGuid);
        else if (userRole == "Teacher") course.Teachers.Add(userGuid);
        else return Unauthorized("Role not recognized.");

        await _courseRepository.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet]
    public IActionResult Create() => View();

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create(string courseName)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var course = new Course
        {
            Name = courseName,
            Teachers = new List<Guid> { Guid.Parse(userId) }
        };

        _courseRepository.AddCourse(course);
        await _courseRepository.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Leave(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null) return NotFound();

        var userGuid = Guid.Parse(userId);

        if (course.Students.Contains(userGuid))
            course.Students.Remove(userGuid);
        else if (course.Teachers.Contains(userGuid))
            course.Teachers.Remove(userGuid);
        else
            return BadRequest("User not enrolled.");

        await _courseRepository.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
