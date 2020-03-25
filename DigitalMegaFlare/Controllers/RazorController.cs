using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DigitalMegaFlare.Data;
using Microsoft.AspNetCore.Hosting;
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
        private readonly ApplicationDbContext _db;

        public RazorController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public ActionResult Load(string razorId)
        {
            var data = _db.RazorFiles.First(x => x.Id == long.Parse(razorId));

            var result = "";
            using (var stream = new MemoryStream(data.Razor))
            {
                result = Encoding.UTF8.GetString(stream.ToArray());
            }
            return Ok(new { Message = $"{result}" });
        }
    }
}