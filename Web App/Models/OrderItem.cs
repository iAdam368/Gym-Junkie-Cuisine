using System.ComponentModel.DataAnnotations;

namespace Web_App.Models
{
    public class OrderItem
    {
        [Required]
        public int OrderNo { get; set; }
        [Required]
        public int StockID { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
