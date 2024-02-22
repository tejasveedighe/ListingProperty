using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListingProperty.Models
{
    public class FavoriteProperty
    {
        [Key]
        public int FavoriteId { get; set; }

        
        public int UserId { get; set; }

        
        public int PropertyId { get; set; }

        public User Users { get; set; }
        public Property Property { get; set; }
    }
}
