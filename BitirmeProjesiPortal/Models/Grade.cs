namespace BitirmeProjesiPortal.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int GradeBy100 { get; set; }
        public string? Comment { get; set; }
        public int ClassReferenceId { get; set; }
        public virtual ClassReference? ClassReference { get; set; }
        public int StudentId { get; set; }
        public virtual UserAccount? Student { get; set; }
        public int ExamTypeId { get; set; }
        public virtual ExamType? ExamType { get; set; }

    }
}
