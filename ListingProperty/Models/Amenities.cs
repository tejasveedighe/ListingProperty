using Azure.Identity;

namespace ListingProperty.Models
{
    public class Amenities
    {
        public int Id { get; set; }
        public bool SwimmingPool { get; set; }
        public bool Parking { get; set; }
        public bool Lifts { get; set; }
        public bool Temple { get; set; }
        public bool RooftopAccess { get; set; }
        public bool Parks { get; set; }
    }
}
