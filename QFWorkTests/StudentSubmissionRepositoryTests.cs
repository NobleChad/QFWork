using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models.Classes;
using QFWork.Models;

namespace QFWorkTests
{
    public class StudentSubmissionRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentSubmissionRepository _repository;

        public StudentSubmissionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new StudentSubmissionRepository(_context);
        }

        [Fact]
        public async Task GetSubmissionsByAssignmentIdAsync_ReturnsSubmissions()
        {
            // Arrange
            var assignmentId = 1;
            _context.StudentSubmissions.Add(new StudentSubmission { Id = 1, AssignmentId = assignmentId });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSubmissionsByAssignmentIdAsync(assignmentId);

            // Assert
            Assert.Single(result);
            Assert.Equal(assignmentId, result.First().AssignmentId);
        }

        [Fact]
        public async Task GetSubmissionByIdAsync_ReturnsSubmission()
        {
            // Arrange
            var submissionId = 1;
            _context.StudentSubmissions.Add(new StudentSubmission { Id = submissionId });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSubmissionByIdAsync(submissionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(submissionId, result.Id);
        }

        [Fact]
        public void AddSubmission_AddsSubmission()
        {
            // Arrange
            var submission = new StudentSubmission { Id = 1, AssignmentId = 1 };

            // Act
            _repository.AddSubmission(submission);

            // Assert
            Assert.Contains(submission, _context.StudentSubmissions);
        }
    }

}
