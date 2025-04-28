using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models.Interfaces;

namespace QFWork.Models.Classes
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "ApplicationDbContext cannot be null.");
        }

        public async Task<IEnumerable<Course>> GetUserCoursesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            return await _context.Courses
                .Where(c => c.Teachers.Contains(userId) || c.Students.Contains(userId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Course ID must be greater than zero.", nameof(id));
            }

            // Return null if no course is found
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course?> GetCourseByPublicKeyAsync(Guid publicKey)
        {
            if (publicKey == Guid.Empty)
            {
                throw new ArgumentException("Public key cannot be empty.", nameof(publicKey));
            }

            // Return null if no course is found
            return await _context.Courses.FirstOrDefaultAsync(c => c.PublicKey == publicKey);
        }

        public void AddCourse(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course), "Course cannot be null.");
            }

            _context.Courses.Add(course);
        }

        public async Task SaveChangesAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            await _context.SaveChangesAsync();
        }
    }
}
