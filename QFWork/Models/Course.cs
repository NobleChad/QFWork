

namespace QFWork.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid PublicKey { get; set; }

        public Course()
        {
            PublicKey = Guid.NewGuid(); 
        }

        public List<Guid> Teachers { get; set; } = new List<Guid>();

        public List<Guid> Students { get; set; } = new List<Guid>();
    }
}
