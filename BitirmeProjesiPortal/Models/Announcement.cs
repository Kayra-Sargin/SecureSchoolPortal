using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeProjesiPortal.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }
        public int ClassReferenceId { get; set; }
        public virtual ClassReference? ClassReference { get; set; }
    }
}
