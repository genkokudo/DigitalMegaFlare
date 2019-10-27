using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;

namespace DigitalMegaFlare.Pages.TestPages
{
    public class DeleteModel : PageModel
    {
        private readonly DigitalMegaFlare.Data.ApplicationDbContext _context;

        public DeleteModel(DigitalMegaFlare.Data.ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TestData = await _context.TestDatas.FindAsync(id);

            if (TestData != null)
            {
                _context.TestDatas.Remove(TestData);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
