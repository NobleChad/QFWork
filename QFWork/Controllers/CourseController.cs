using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QFWork.Atributes;
using QFWork.Models;
using QFWork.Models.Interfaces;
using System.Security.Claims;

namespace QFWork.Controllers
{
    [Authorize]
    [UserInCourse]
    public class CourseController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IStudentSubmissionRepository _submissionRepository;

        public CourseController(IUserRepository userRepository,
            ICourseRepository courseRepository,
            IAssignmentRepository assignmentRepository,
            IStudentSubmissionRepository submissionRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
        }

        public async Task<IActionResult> Details(int id)
        {
            var assignments = await _assignmentRepository.GetAssignmentsByCourseIdAsync(id);
            ViewData["CourseId"] = id;
            return View(assignments);
        }
        [HttpGet]
        public async Task<IActionResult> Users(int id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(user);

            // Assuming a repository or service exists for accessing courses
            var course = await _courseRepository.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            // Check if the user is part of the course
            if (!course.Students.Contains(userId) && !course.Teachers.Contains(userId))
            {
                return Unauthorized();
            }

            // Retrieve users associated with the course
            var userGuids = course.Students.Concat(course.Teachers).ToList();
            var users = await _userRepository.GetUsersByIdsAsync(userGuids);

            return View(users);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult CreateTask(int id) => View();

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateTask(int id, Assignment assignment)
        {
            assignment.CourseId = id;
            assignment.AssignmentId = 0;
            _assignmentRepository.AddAssignment(assignment);
            await _assignmentRepository.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Assignment(int id)
        {
            var assignment = await _assignmentRepository.GetAssignmentByIdAsync(id);
            if (assignment == null) return NotFound();

            assignment.studentSubmissions = (await _submissionRepository.GetSubmissionsByAssignmentIdAsync(id)).ToList();
            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAssignment(int assignmentId, IFormFile file)
        {
            if (file == null || Path.GetExtension(file.FileName) != ".txt")
            {
                ModelState.AddModelError("File", "Only .txt files are allowed.");
                return RedirectToAction("Details", new { id = Request.Cookies["CourseId"] });
            }

            var uploadsPath = Path.Combine("wwwroot/uploads");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, file.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var submission = new StudentSubmission
            {
                AssignmentId = assignmentId,
                StudentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                FilePath = "/uploads/" + file.FileName,
                SubmittedAt = DateTime.UtcNow,
                Grade = "Pending"
            };

            _submissionRepository.AddSubmission(submission);
            await _submissionRepository.SaveChangesAsync();

            return RedirectToAction("Details", new { id = Request.Cookies["CourseId"] });
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> SetGrade(int submissionId, int assignmentId, string grade)
        {
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null) return NotFound();

            submission.Grade = grade;
            await _submissionRepository.SaveChangesAsync();

            return RedirectToAction("Assignment", new { id = assignmentId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTask(int assignmentId)
        {
            var assignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
            if (assignment == null) return NotFound();

            _assignmentRepository.RemoveAssignment(assignment);
            await _assignmentRepository.SaveChangesAsync();

            return RedirectToAction("Details", new { id = assignment.CourseId });
        }
    }
}
