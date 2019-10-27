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
    public class DetailsModel : PageModel
    {
        private readonly DigitalMegaFlare.Data.ApplicationDbContext _context;

        public DetailsModel(DigitalMegaFlare.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
