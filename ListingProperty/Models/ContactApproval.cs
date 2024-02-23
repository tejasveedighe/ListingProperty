using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class ContactApproval
    {
        [Key]
        public int ContactApprovalId { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public bool ApprovalStatus { get; set; }
        public  DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public User User { get; set; }
        public Property Property { get; set; }
    }
}
