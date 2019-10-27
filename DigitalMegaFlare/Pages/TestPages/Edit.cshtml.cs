using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;

namespace DigitalMegaFlare.Pages.TestPages
{
    public class EditModel : PageModel
    {
        private readonly DigitalMegaFlare.Data.ApplicationDbContext _context;

        public EditModel(DigitalMegaFlare.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TestData TestData { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TestData = await _context.TestDatas.FirstOrDefaultAsync(m => m.Id == id);

            if (TestData == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TestData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestDataExists(TestData.Id))
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

        private bool TestDataExists(long id)
        {
            return _context.TestDatas.Any(e => e.Id == id);
        }
    }
}
