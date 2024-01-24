using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages.Menu
{
    public class EditModel : PageModel
    {
        private readonly Web_App.Data.Web_AppContext _context;

        public EditModel(Web_App.Data.Web_AppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FoodItem FoodItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FoodItems == null)
            {
                return NotFound();
            }

            var fooditems =  await _context.FoodItems.FirstOrDefaultAsync(m => m.FoodID == id);
            if (fooditems == null)
            {
                return NotFound();
            }
            FoodItem = fooditems;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var file in Request.Form.Files)
            {
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);

                if (_context.FoodItems != null)
                {
                    FoodItem.ImageData = ms.ToArray();
                    _context.FoodItems.Update(FoodItem);
                }

                ms.Close();
                ms.Dispose();
            }



            


            _context.Attach(FoodItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodItemsExists(FoodItem.FoodID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FoodItemsExists(int id)
        {
          return _context.FoodItems.Any(e => e.FoodID == id);
        }
    }
}
