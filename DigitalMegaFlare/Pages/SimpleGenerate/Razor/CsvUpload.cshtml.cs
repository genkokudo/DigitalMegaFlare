using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalMegaFlare.Pages.SimpleGenerate.Razor
{
    public class CsvUploadModel : PageModel
    {
        public void OnGet()
        {

        }

        public ActionResult OnPostUpload(IFormFile file)
        {
            // アップロード処理
            // 一時ファイルのパスを取得
            var filePath = Path.GetTempFileName();
            // 各ファイルについて、ストリームを作成して
            // その一時ファイルパスにコピー
            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return Page();
        }
    }
}