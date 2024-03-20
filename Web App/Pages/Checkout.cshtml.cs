using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages
{
    public class CheckoutModel : PageModel
    {

        private readonly Web_AppContext _db;
        private readonly UserManager<IdentityUser> _UserManager;
        public IList<CheckoutItem> Items { get; set; }

        public decimal Total;
        public long AmountPayable;
        
        public CheckoutModel(Web_AppContext db, UserManager<IdentityUser> UserManager)
        {
            _db = db;
            _UserManager = UserManager;
        }   
        
        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            // Renamed 'FoodName' column to 'Item_Name' for mapping to the CheckoutItems class
            Items = _db.CheckoutItems.FromSqlRaw(
                "SELECT FoodItem.FoodID AS ID, FoodItem.Price, " +
                "FoodItem.FoodName AS Item_Name, " +
                "BasketItems.BasketID, BasketItems.Quantity " +
                "FROM FoodItem INNER JOIN BasketItems " +
                "ON FoodItem.FoodID = BasketItems.StockID " +
                "WHERE BasketID = {0}", customer.BasketID)
            .ToList();

            Total = 0;

            foreach (var item in Items)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)Total;
        }


    }
}
