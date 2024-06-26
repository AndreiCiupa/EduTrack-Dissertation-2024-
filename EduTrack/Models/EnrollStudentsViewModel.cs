using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTrack.Models
{
    public class EnrollStudentsViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<SelectListItem> Subjects { get; set; } = new List<SelectListItem>();
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();

        // Adăugăm pentru profesori
        public List<SelectListItem> Teachers { get; set; } = new List<SelectListItem>();
        public int SelectedTeacherId { get; set; }
    }
}
