using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace DigitalMegaFlare.Pages.SimpleGenerate.Razor
{
    [Authorize]
    public class CsvLaboModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public CsvLaboModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void OnGet()
        {
            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files");

            using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo("file.csv");

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    // TODO: fileInfo.PhysicalPath がフルパスになるので、これに対して処理を行う
                }
                else
                {
                    ViewData["Error"] = "ファイルが存在しません。";
                }
            }

            //// 改行コード、コンマで分けて格納する
            //var data = File.ReadAllText(filePath);
            //data = data.Replace("\r\n", "\n");
            //data = data.Replace("\r", "\n");
            //data = data.Trim('\n');
            //var lines = data.Split('\n');
            //List<string[]> splitedLines = new List<string[]>();
            //foreach (var item in lines)
            //{
            //    splitedLines.Add(item.Split(','));
            //}

            //RawCsv = splitedLines.ToArray();
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
                    //file.CopyTo(stream);
                }
            }
            return Page();
        }


    }
}