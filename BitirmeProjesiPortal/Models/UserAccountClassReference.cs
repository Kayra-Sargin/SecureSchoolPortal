namespace BitirmeProjesiPortal.Models
{
    public class UserAccountClassReference
    {
        public int Id { get; set; }
        public int ClassReferenceId { get; set; }
        public virtual ClassReference? ClassReference { get; set; }
        public int UserId { get; set; }
        public virtual UserAccount? User { get; set; }

    }
}
