using System.ComponentModel.DataAnnotations;

namespace QFWork.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public List<StudentSubmission> studentSubmissions { get; set; } = new List<StudentSubmission>();
    }
}
