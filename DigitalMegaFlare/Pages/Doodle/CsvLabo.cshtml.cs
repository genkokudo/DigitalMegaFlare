using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalMegaFlare
{
    public class CsvLaboModel : PageModel
    {
        public void OnGet()
        {

        }

        public void Upload(IFormFile file)
        {
            if (ModelState.IsValid)
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
                        //file.CopyTo(stream);
                    }
                }
            }
        }
    }
}