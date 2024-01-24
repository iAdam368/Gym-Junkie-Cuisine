using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages.Menu
{
    public class CreateModel : PageModel
    {
        private readonly Web_App.Data.Web_AppContext _context;



        [BindProperty]
        public FoodItem FoodItem { get; set; }



        public CreateModel(Web_App.Data.Web_AppContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }


        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }



        byte[] bytes = null;

        if (FoodItem.ImageData != null) 
        {
            using (Stream fs = FoodItem.ImageData.OpenReadStream())
            {
                using(BinaryReader br = new BinaryReader(fs)) 
                {
                    bytes = br.ReadBytes((Int32)fs.Length);
                }
            }
            FoodItem.ImageDataAsBase64 = Convert.ToBase64String(bytes, 0, bytes.Length);
        }
        _context.FoodItems.Add(FoodItem);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");


            /*
          foreach (var file in Request.Form.Files)
            {
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                FoodItem.ImageData = ms.ToArray();
                
                ms.Close();
                ms.Dispose();
            }

            _context.FoodItems.Add(FoodItem);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index"); */
        }
    }
}
