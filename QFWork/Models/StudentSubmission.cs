using System.ComponentModel.DataAnnotations;

namespace QFWork.Models
{
    public class StudentSubmission
    {
        [Key]
        public int Id { get; set; }
        public int AssignmentId { get; set; } 
        public Guid StudentId { get; set; } 
        public string FilePath { get; set; } 
        public DateTime SubmittedAt { get; set; }
        public string Grade { get; set; } 

        public Assignment Assignment { get; set; }
    }
}