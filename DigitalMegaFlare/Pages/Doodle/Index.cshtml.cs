using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public string Test { get; set; }

        public IActionResult OnGet()
        {
            //Data = await _mediator.Send(new Query { Id = 1 });
            return Page();
        }

        public IActionResult OnPost()
        {
            ViewData["Message"] = "ボタンを押しますたm9(･Å･)";
            return Page();
        }
    }

}