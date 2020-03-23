using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mintea.HtmlToDom;

namespace DigitalMegaFlare.Pages.Botsu
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
            try
            {
                ViewData["Output"] = Trans.ToJQuery(Input);
            }catch(Exception e)
            {
                ViewData["ErrorMessage"] = e.Message;
            }
            return Page();
        }

    }
}