using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QFWork.Atributes;
using QFWork.Data;
using QFWork.Models;
using System.Security.Claims;

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
        public async Task<IActionResult> Details(int id)
        {
            var assignments = await _context.Assignments
                .Where(a => a.CourseId == id)
                .ToListAsync();

            ViewData["CourseId"] = id;
            return View(assignments);
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult CreateTask(int id)
        {
            return View();
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult CreateTask(int id, Assignment assignment)
        {
            assignment.CourseId = id;
            assignment.AssignmentId = 0;
            _context.Assignments.Add(assignment);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = Request.Cookies["CourseId"] });
        }
        public IActionResult Assignment(int id)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id);
            assignment.studentSubmissions = _context.StudentSubmissions.Where(x => x.AssignmentId == assignment.AssignmentId).ToList() ?? new List<StudentSubmission>();

            return View(assignment);
        }
        [HttpPost]
        public IActionResult UploadAssignment(int assignmentId, IFormFile file)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension != ".txt")
                {
                    ModelState.AddModelError("File", "Only .txt files are allowed.");
                    return RedirectToAction("Details", new { id = Request.Cookies["CourseId"] });
                }

                var filePath = Path.Combine("wwwroot/uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var submission = new StudentSubmission
                {
                    AssignmentId = assignmentId,
                    StudentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    FilePath = "/uploads/" + file.FileName,
                    SubmittedAt = DateTime.Now,
                    Grade = "Pending"
                };

                _context.StudentSubmissions.Add(submission);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = Request.Cookies["CourseId"] });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> SetGrade(int submissionId, int assignmentId, string grade)
        {
            var submission = await _context.StudentSubmissions
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                return NotFound();
            }

            submission.Grade = grade;

            await _context.SaveChangesAsync();

            return RedirectToAction("Assignment", new { id = assignmentId });
        }
        public IActionResult Users(int id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [HttpPost]
        public IActionResult RemoveTask(int assignmentId)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = assignment.CourseId });
        }
    }
}
