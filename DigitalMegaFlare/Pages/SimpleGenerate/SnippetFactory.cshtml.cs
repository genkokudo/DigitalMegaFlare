using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalMegaFlare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalMegaFlare.Pages.SimpleGenerate
{
    public class SnippetFactoryModel : PageModel
    {
        [BindProperty]
        public Snippet Input { get; set; }

        [BindProperty]
        public string Output { get; set; }

        public IActionResult OnGetAsync()
        {
            // TODO:クッキーから読めないかなあー
            Input = new Snippet();
            return Page();
        }

        public IActionResult OnPostGenerateXmlAsync()
        {
            return Page();
        }

        public IActionResult OnPostDownloadAsync()
        {
            return Page();
        }
    }
}