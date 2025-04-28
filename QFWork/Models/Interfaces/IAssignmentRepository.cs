namespace QFWork.Models.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseIdAsync(int courseId);
        Task<Assignment> GetAssignmentByIdAsync(int id);
        void AddAssignment(Assignment assignment);
        void RemoveAssignment(Assignment assignment);
        Task SaveChangesAsync();
    }
}
