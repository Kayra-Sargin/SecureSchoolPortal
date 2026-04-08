namespace BitirmeProjesiPortal.Models
{
    public class ClassReference
    {
        public int Id { get; set; }
        public string? CRN { get; set; }
        public int ClassId { get; set; }
        public virtual Class? Class { get; set; }
        public int UserId { get; set; }
        public virtual UserAccount? User { get; set; }
    }
}
