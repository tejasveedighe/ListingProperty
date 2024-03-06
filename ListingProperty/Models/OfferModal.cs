using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class OfferModal
    {
        [Key]
        public int OfferId { get; set; }
        public int OfferPrice { get; set; }
        public string OfferText { get; set; }
        public DateTime OfferLastDate { get; set; }
        public bool SellerApproved { get; set; }
        public bool AdminApproved { get; set; }
        public int OfferCompleted { get; set; }

    }
}
