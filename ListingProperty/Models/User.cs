using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListingProperty.Models
{
    public class User
    {

        [Key]
        public int UserId { get; set; }

       

        [Required]
        public string Name { get; set; }


        [Required(ErrorMessage = "Required proper Email context ")]
        [EmailAddress]
        public string Email { get; set; } 


        [Required(ErrorMessage = "Password must be max them 5 character")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)] 
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UserType { get; set; } = "Admin";


        public FavoriteProperty FavoriteProperty { get; set; } 

    }
}
