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
using Microsoft.Extensions.Hosting;

namespace DigitalMegaFlare.Pages.Botsu
{
    public class CsvUploadModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public CsvUploadModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void OnGet()
        {

        }

        public async Task<ActionResult> OnPostUploadAsync(IFormFile file)
        {

            // アップロードされたファイルをサーバに保存する
            using (var fileStream = file.OpenReadStream())
            {
                var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory);

                using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
                {
                    // ファイル情報を取得
                    IFileInfo fileInfo = provider.GetFileInfo("file.csv");

                    using (var stream = new FileStream(fileInfo.PhysicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            return Page();
        }
    }
}