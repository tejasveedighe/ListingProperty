namespace ListingProperty.Models
{
    public class PropertyResponseDTO
    {
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int NoBedroom { get; set; }
        public int NoBathroom { get; set; }
        public int SquareFeet { get; set; }
        public string Description { get; set; }
        public ICollection<ImageDTO> Images { get; set; }
        public long ContactNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateListed { get; set; }
        public DateTime DateUpdated { get; set; }
        public FavoriteProperty FavoriteProperty { get; set; }
    }

    public class ImageDTO
    {
        public int Id { get; set; }
        public string PublicID { get; set; }
        public string ImageUrl { get; set; }
        public int PropertyId { get; set; }
    }
}
