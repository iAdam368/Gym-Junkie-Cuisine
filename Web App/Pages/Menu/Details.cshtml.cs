using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages.Menu
{
    public class DetailsModel : PageModel
    {
        private readonly Web_App.Data.Web_AppContext _context;

        public DetailsModel(Web_App.Data.Web_AppContext context)
        {
            _context = context;
        }

      public FoodItem FoodItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FoodItems == null)
            {
                return NotFound();
            }

            var fooditems = await _context.FoodItems.FirstOrDefaultAsync(m => m.FoodID == id);
            if (fooditems == null)
            {
                return NotFound();
            }
            else 
            {
                FoodItem = fooditems;
            }
            return Page();
        }
    }
}
