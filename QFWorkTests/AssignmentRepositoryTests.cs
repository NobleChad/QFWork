using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models;
using QFWork.Models.Classes;

namespace QFWorkTests
{

    public class AssignmentRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AssignmentRepository _repository;

        public AssignmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new AssignmentRepository(_context);
            _context.Assignments.Add(new Assignment { AssignmentId = 1, CourseId = 1, Title = "Test Assignment", Description = "Test" });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAssignmentsByCourseIdAsync_ReturnsAssignments()
        {
            // Arrange
            var courseId = 1;

            // Act
            var result = await _repository.GetAssignmentsByCourseIdAsync(courseId);

            // Assert
            Assert.Single(result);
            Assert.Equal(courseId, result.First().CourseId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ReturnsAssignment()
        {
            // Arrange
            var assignmentId = 1;

            // Act
            var result = await _repository.GetAssignmentByIdAsync(assignmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assignmentId, result.AssignmentId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetAssignmentByIdAsync(999));
        }

        [Fact]
        public async Task RemoveAssignment_RemovesAssignment()
        {
            // Arrange
            var assignment = _context.Assignments.First(a => a.AssignmentId == 1);

            // Act
            _repository.RemoveAssignment(assignment);
            await _repository.SaveChangesAsync();

            // Assert
            Assert.DoesNotContain(assignment, _context.Assignments);
        }
    }

}