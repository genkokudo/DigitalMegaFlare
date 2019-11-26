using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mintea.HtmlToDom;

namespace DigitalMegaFlare.Pages.SimpleGenerate
{
    public class HtmlToJqueryModel : PageModel
    {
        [BindProperty]
        public string Input { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnPostTrans()
        {
            ViewData["Output"] = Trans.ToJQuery(Input);
            return Page();
        }

    }
}