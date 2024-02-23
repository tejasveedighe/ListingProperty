using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ListingProperty.ViewModal
{
    public class ContactApproverDTO
    {

        [Required]
        [Range (1, int.MaxValue)]
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int PropertyId { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
