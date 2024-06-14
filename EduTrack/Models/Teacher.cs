using System.ComponentModel.DataAnnotations;

namespace EduTrack.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }


        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public List<Mark> Marks { get; set; } = new List<Mark>();
    }
}
