using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMegaFlare.Controllers
{
    /// <summary>
    /// テスト実装のコントローラーです。
    /// </summary>
    //[Authorize]
    public class DoodleController : Controller
    {

        public ActionResult Download(long id)
        {
            return Ok();
            //return File(new byte[], "application/pdf", $"stampHistory{id}.pdf");
        }
    }
}