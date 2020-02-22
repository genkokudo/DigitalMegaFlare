using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using RazorLight;

namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public IndexModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string Test { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var engine = new RazorLightEngineBuilder()
                          .UseEmbeddedResourcesProject(typeof(Program))
                          .UseMemoryCachingProvider()
                          .DisableEncoding()
                          .Build();

            // テンプレート読み込み
            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files", "razors", "asp");

            using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo("Model.dat");

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    var template = System.IO.File.ReadAllText(fileInfo.PhysicalPath);

                    //var template = "Name: @Model.Name, Age: @Model.Age";
                    var Project = new { Name = "DigitalMegaFlare" };
                    var ModelList = new { Name = "Test", Comment = "テストモデル" };
                    var model = new { Project = Project, ModelList = ModelList };

                    string result = await engine.CompileRenderStringAsync("templateKey", template, model);

                    ViewData["Message"] = result;
                }
                else
                {
                    ViewData["Error"] = "ファイルが存在しません。";
                }
            }

            return Page();

//            ■匿名型の動的作成 
            //dynamic tmp2 = new ExpandoObject(); 
            //var dic = new Dictionary<string, object>() { { "name", "hogehoge" }, { "age", 10 } }; 
 
            //IDictionary<string, object> wk = tmp2; 
            //foreach (var item in dic) { wk.Add(item.Key, item.Value); } 
 
            //Console.WriteLine("----- tmp2 -----"); 
            //Console.WriteLine(tmp2.name); 
            //Console.WriteLine(tmp2.age); 
        }
    }

}