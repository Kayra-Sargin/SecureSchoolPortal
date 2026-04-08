using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeProjesiPortal.Models
{
    public class ClassFile
    {
        public int Id { get; set; }
        public DateTime UploadDate { get; set; }
        public string? FilePath { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public int ClassReferenceId { get; set; }
        public virtual ClassReference? ClassReference { get; set; }

    }
}
