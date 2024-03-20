using System.ComponentModel.DataAnnotations;

namespace Web_App.Models
{
    public class CheckoutItem
    {
        [Key, Required]
        public int ID { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Item_Name { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

}
