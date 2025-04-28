using Microsoft.EntityFrameworkCore;
using QFWork.Data;
using QFWork.Models.Interfaces;

namespace QFWork.Models.Classes
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByCourseIdAsync(int courseId)
        {
            return await _context.Assignments.Where(a => a.CourseId == courseId).ToListAsync();
        }

        public async Task<Assignment> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with ID {id} was not found.");
            }
            return assignment;
        }

        public void AddAssignment(Assignment assignment)
        {
            _context.Assignments.Add(assignment);
        }

        public void RemoveAssignment(Assignment assignment)
        {
            _context.Assignments.Remove(assignment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
