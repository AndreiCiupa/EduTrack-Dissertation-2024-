namespace EduTrack.Models
{
    internal class StudentMarksViewModel
    {
        public string StudentName { get; set; }
        public double AverageMark { get; set; }
        public List<Mark> Marks { get; set; }
    }
}