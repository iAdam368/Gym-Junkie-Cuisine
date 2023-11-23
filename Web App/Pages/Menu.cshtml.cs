using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet()
        {
            FoodItem = _context.FoodItems.FromSqlRaw("Select * FROM FoodItem").ToList();
        }

    }
}
