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
    public class IndexModel : PageModel
    {
        private readonly DigitalMegaFlare.Data.ApplicationDbContext _context;

        public IndexModel(DigitalMegaFlare.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<TestData> TestData { get;set; }

        public async Task OnGetAsync()
        {
            TestData = await _context.TestDatas.ToListAsync();
        }
    }
}
