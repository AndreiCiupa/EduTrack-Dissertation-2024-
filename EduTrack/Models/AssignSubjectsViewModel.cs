﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTrack.Models
{
    public class AssignSubjectsViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public List<SelectListItem> Subjects { get; set; } = new List<SelectListItem>();
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();
    }
}
