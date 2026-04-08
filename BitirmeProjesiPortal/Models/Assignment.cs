using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeProjesiPortal.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public string? AnswerFilePath { get; set; }
        [NotMapped]
        public IFormFile? AnswerFile { get; set; }
        public int ClassReferenceId { get; set; }
        public virtual ClassReference? ClassReference { get; set; }
    }
}
