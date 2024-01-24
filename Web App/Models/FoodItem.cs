using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_App.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodID { get; set; }
        [StringLength(100)]
        public string FoodName { get; set; }
        [StringLength(255)]
        public string FoodDescription { get; set; }
        [StringLength(20)]
        public string CategoryOfFood { get; set; }
        [StringLength(20)]
        public string Specification { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Nullable<decimal> Price { get; set; }
        [StringLength(255)]
        public string ImageDescription { get; set; }
        [NotMapped]
        public IFormFile ImageData { get; set; }
        public string ImageDataAsBase64 { get; set; } // Property to store the Base64 representation of ImageData 

    }
}