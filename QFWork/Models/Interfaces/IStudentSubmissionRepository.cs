namespace QFWork.Models.Interfaces
{
    public interface IStudentSubmissionRepository
    {
        Task<IEnumerable<StudentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<StudentSubmission?> GetSubmissionByIdAsync(int submissionId);
        void AddSubmission(StudentSubmission submission);
        Task SaveChangesAsync();
    }
}
