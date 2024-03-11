using ListingProperty.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public string PropertyTitle { get; set; }
        [Required(ErrorMessage = "Select Property type")]
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int NoBedroom { get; set; }
        public int NoBathroom { get; set; }
        public int SquareFeet { get; set; }
        public string Description { get; set; }
        /* public List<IFormFile> Images { get; set; }*/
        public ICollection<Image> Images { get; set; }
        public long ContactNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateListed { get; set; }
        public DateTime DateUpdated { get; set; }
        public Boolean Approved { get; set; }
        public FavoriteProperty FavoriteProperty { get; set; }
        public Amenities? Amenities { get; set; }
    }
}
