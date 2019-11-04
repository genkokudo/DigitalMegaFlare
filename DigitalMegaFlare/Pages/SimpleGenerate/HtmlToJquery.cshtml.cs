using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlToDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalMegaFlare.Pages.SimpleGenerate
{
    public class HtmlToJqueryModel : PageModel
    {
        [BindProperty]
        public string Input { get; set; }
        [BindProperty]
        public string Output { get; set; }
        public void OnGet()
        {

        }
        public void OnTrans()
        {
            Output = Trans.ToJQuery(Input);
        }
    }
}