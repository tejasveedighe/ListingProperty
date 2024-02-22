using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ListingProperty.Models
{
    public class Image
    {


        [Key]
        public int Id { get; set; }

   
        public string PublicID { get; set; }

       
        public string ImageUrl { get; set; }
        public int PropertyId { get; set; }
        //public Property Property { get; set; }


    }
}
