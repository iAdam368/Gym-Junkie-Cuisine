using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages
{
    public class MenuModel : PageModel
    {
        private readonly Web_AppContext _context;
        public IList<FoodItem> FoodItem { get; set; } = default!;

        public MenuModel(Web_AppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Search { get; set; }

        public void OnGet()
        {
            FoodItem = _context.FoodItems.FromSqlRaw("Select * FROM FoodItem").ToList();
        }

        public IActionResult OnPostSearch()
        {
            // Adjusted code to prevent SQL injection (previously an exception was thrown when entering ')
            string queryString = "SELECT * FROM FoodItem WHERE FoodName LIKE @Search";
            SqlParameter parameter = new SqlParameter("@Search", "%" + Search + "%");
            FoodItem = _context.FoodItems.FromSqlRaw(queryString, parameter).ToList();
            return Page();
        }
    }
}
