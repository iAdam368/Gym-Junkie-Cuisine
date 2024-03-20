using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Models;
using Microsoft.AspNetCore.Identity;

namespace Web_App.Pages
{
    public class MenuModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager; // Added for the basket feature 
        private readonly Web_AppContext _context;
        public IList<FoodItem> FoodItem { get; set; } = default!;
        [BindProperty]
        public string Search { get; set; }


        // Modified for importing the identity and using the userManager 
        public MenuModel(Web_AppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            OnGet();
        }



        public void OnGet()
        {
            try
            {
                FoodItem = _context.FoodItems.FromSqlRaw("Select * FROM FoodItem").ToList();
            }
            catch (SqlException)
            {
                // SQL Database Exception
            }
        }

        public IActionResult OnPostSearch()
        {
            // Adjusted code to prevent SQL injection (previously an exception was thrown when entering ')
            string queryString = "SELECT * FROM FoodItem WHERE FoodName LIKE @Search";
            SqlParameter parameter = new SqlParameter("@Search", "%" + Search + "%");
            FoodItem = _context.FoodItems.FromSqlRaw(queryString, parameter).ToList();

            return Page();
        }


        // New method for accepting the foodID 
        // Checking for baskets and incrementing quanitity appropriately 
        // Updating the database also 
        public async Task OnPostBuyAsync(int foodID)
        {
            var user = await _userManager.GetUserAsync(User);

            CheckoutCustomer customer = await _context
                .CheckoutCustomers
                .FindAsync(user.Email);

            var item = _context.BasketItems
                .FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", foodID, customer.BasketID)
                .ToList()
                .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    StockID = foodID,
                    Quantity = 1
                };
                _context.BasketItems.Add(newItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _context.Attach(item).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found exception ", e);
                }
            }

        }


    }
}
