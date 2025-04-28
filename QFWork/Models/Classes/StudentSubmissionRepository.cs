using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models.Interfaces;

namespace QFWork.Models.Classes
{
    public class StudentSubmissionRepository : IStudentSubmissionRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentSubmissionRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "ApplicationDbContext cannot be null.");
        }

        public async Task<IEnumerable<StudentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            if (assignmentId <= 0)
            {
                throw new ArgumentException("Assignment ID must be greater than zero.", nameof(assignmentId));
            }

            return await _context.StudentSubmissions
                .Where(x => x.AssignmentId == assignmentId)
                .ToListAsync();
        }

        public async Task<StudentSubmission?> GetSubmissionByIdAsync(int submissionId)
        {
            if (submissionId <= 0)
            {
                throw new ArgumentException("Submission ID must be greater than zero.", nameof(submissionId));
            }

            // Return null if no submission is found
            return await _context.StudentSubmissions.FindAsync(submissionId);
        }

        public void AddSubmission(StudentSubmission submission)
        {
            if (submission == null)
            {
                throw new ArgumentNullException(nameof(submission), "Submission cannot be null.");
            }

            _context.StudentSubmissions.Add(submission);
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
