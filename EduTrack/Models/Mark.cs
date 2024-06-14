using System.ComponentModel.DataAnnotations;

namespace EduTrack.Models
{
    public class Mark
    {
        public int Id { get; set; }
        [Range(1, 10)]
        public int Value { get; set; }
        public DateTime DateTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}
