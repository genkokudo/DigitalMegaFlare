using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMegaFlare.Controllers
{
    /// <summary>
    /// Razor実験室のコントローラーです。
    /// </summary>
    //[Authorize]
    public class RazorController : Controller
    {

        public ActionResult Load(string filepath)
        {
            var data = System.IO.File.ReadAllText(filepath);
            return Ok(new { Message = $"{data}" });
        }

        // using Microsoft.AspNetCore.Mvc
        public ActionResult Test(IFormFile file)
        {
            return Ok();
            //return File(new byte[], "application/pdf", $"stampHistory{id}.pdf");
        }
    }
}