using ListingProperty.Enums;
using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class Payments
    {
        [Key]
        public int PaymentId { get; set; }
        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public decimal Price { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
