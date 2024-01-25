using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_App.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "The food name is required.")]
        public string FoodName { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "The food description is required.")]
        public string FoodDescription { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The category of food is required.")]
        public string CategoryOfFood { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "The food specification is required.")]
        public string Specification { get; set; }

        [Required(ErrorMessage = "The food availability is required.")]
        public Nullable<bool> IsAvailable { get; set; }

        [Required(ErrorMessage = "The food price is required.")]
        [Range(1, 100, ErrorMessage = "The price must be between £1 and £100.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Nullable<decimal> Price { get; set; }

        [Required(ErrorMessage = "The food description is required.")]
        [StringLength(255)]
        public string ImageDescription { get; set; }

        public Byte[] ImageData { get; set; }
    }
}
