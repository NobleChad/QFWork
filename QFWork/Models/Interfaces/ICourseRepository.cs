namespace QFWork.Models.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetUserCoursesAsync(Guid userId);
        Task<Course?> GetCourseByIdAsync(int id); 
        Task<Course?> GetCourseByPublicKeyAsync(Guid publicKey); 
        void AddCourse(Course course);
        Task SaveChangesAsync();
    }

}
