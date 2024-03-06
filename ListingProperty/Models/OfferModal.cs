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
        public Enums.ApprovalStatus SellerStatus { get; set; }
        public Enums.ApprovalStatus AdminStatus { get; set; }
        public Enums.ApprovalStatus OfferStatus { get; set; }

        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }

    }
}
