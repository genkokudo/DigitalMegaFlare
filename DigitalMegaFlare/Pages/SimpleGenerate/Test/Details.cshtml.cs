using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;

namespace DigitalMegaFlare
{
    public class DetailsModel : PageModel
    {
        private readonly DigitalMegaFlare.Data.ApplicationDbContext _context;

        public DetailsModel(DigitalMegaFlare.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public ExcelInputHistory ExcelInputHistory { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExcelInputHistory = await _context.ExcelInputHistories.FirstOrDefaultAsync(m => m.Id == id);

            if (ExcelInputHistory == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
