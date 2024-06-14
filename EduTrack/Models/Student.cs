using System.ComponentModel.DataAnnotations;

namespace EduTrack.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Range(6, 120)]
        public int Age { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }


        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public List<Mark> Marks { get; set; } = new List<Mark>();
    }
}
