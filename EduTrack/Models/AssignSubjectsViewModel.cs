using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTrack.Models
{
    public class AssignSubjectsViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public List<SelectListItem> Subjects { get; set; }
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}
