using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models;
using QFWork.Models.Classes;

namespace QFWorkTests
{
    public class CourseRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CourseRepository _repository;

        public CourseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new CourseRepository(_context);
        }

        [Fact]
        public async Task GetUserCoursesAsync_ReturnsCourses()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _context.Courses.Add(new Course { Id = 1, Name = "Test Course", Teachers = new List<Guid> { userId } });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserCoursesAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Course", result.First().Name);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ReturnsCourse()
        {
            // Arrange
            var courseId = 1;
            _context.Courses.Add(new Course { Id = courseId, Name = "Test Course" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCourseByIdAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(courseId, result.Id);
        }

        [Fact]
        public async Task GetCourseByPublicKeyAsync_ReturnsCourse()
        {
            // Arrange
            var publicKey = Guid.NewGuid();
            _context.Courses.Add(new Course { Id = 1, Name = "Test Course", PublicKey = publicKey });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCourseByPublicKeyAsync(publicKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(publicKey, result.PublicKey);
        }

        [Fact]
        public async Task AddCourse_AddsCourse()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Test Course" };

            // Act
            _repository.AddCourse(course);
            await _repository.SaveChangesAsync();

            // Assert
            Assert.Contains(course, _context.Courses);
        }
    }

}
