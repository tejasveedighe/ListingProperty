using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class OwnedProperties
    {
        public int Id { get; set; }
        public Property Property { get; set; }
        public User Buyer { get; set; }
        public OfferModal Offer { get; set; }
        public Payments Transaction { get; set; }
    }
}
