using ListingProperty.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ListingProperty.ViewModal
{
    public class AdminRequestDTO
    {

        [Required]
        [Range (1, int.MaxValue)]
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int PropertyId { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}
