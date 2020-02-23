using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using DigitalMegaFlare.Pages.SimpleGenerate.Razor;
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
                    // Razorスクリプト読み込み
                    var template = System.IO.File.ReadAllText(fileInfo.PhysicalPath);

                    // Excelから読み込み
                    var excelDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files", "excels");
                    var excel = ReadExcel(excelDirectry, "Model.xlsx");

                    // Modelの作成
                    var model = CreateModel(excel);

                    // 生成
                    string result = await engine.CompileRenderStringAsync("templateKey", template, model);

                    ViewData["Message"] = result;
                }
                else
                {
                    ViewData["Error"] = "ファイルが存在しません。";
                }
            }

            return Page();

        }

        /// <summary>
        /// Razorに入力するModelを作成する
        /// </summary>
        /// <returns></returns>
        private dynamic CreateModel(ExcelUploadResult excel)
        {

            // TODO:組み立て
            dynamic project = InputDynamic(new Dictionary<string, object>() { { "Name", "DigitalMegaFlare" } });
            dynamic fieldListRow0 = InputDynamic(new Dictionary<string, object>() { { "Name", "Name" }, { "Comment", "名前" }, { "Attribute", "[StringLength(100)]" }, { "Type", "string" } });
            dynamic fieldListRow1 = InputDynamic(new Dictionary<string, object>() { { "Name", "Score" }, { "Comment", "点数" }, { "Attribute", "" }, { "Type", "int" } });
            dynamic fieldList = new List<dynamic> { fieldListRow0, fieldListRow1 };
            dynamic modelList = InputDynamic(new Dictionary<string, object>() { { "Name", "Test" }, { "Comment", "テストモデル" }, { "IsMaster", false }, { "FieldList", fieldList } });
            dynamic model = InputDynamic(new Dictionary<string, object>() { { "Project", project }, { "ModelList", modelList } });

            return model;
        }

        /// <summary>
        /// Excelを読み込む
        /// </summary>
        /// <param name="directry">ディレクトリ</param>
        /// <param name="filename">拡張子付きのファイル名</param>
        /// <returns></returns>
        private ExcelUploadResult ReadExcel(string directry, string filename = "Model.xlsx")
        {
            // ファイルの読み込み
            List<string> sheetNames = new List<string>();
            List<List<List<string>>> xlsx = new List<List<List<string>>>();
            using (PhysicalFileProvider provider = new PhysicalFileProvider(directry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo(filename);

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    using (var wb = new XLWorkbook(fileInfo.PhysicalPath))
                    {
                        foreach (var ws in wb.Worksheets)
                        {
                            // ワークシート
                            List<List<string>> sheet = new List<List<string>>();

                            // シート名を取得
                            sheetNames.Add(ws.Name);

                            for (int i = 1; i <= ws.LastCellUsed().Address.RowNumber; i++)
                            {
                                List<string> raw = new List<string>();
                                for (int j = 1; j <= ws.LastCellUsed().Address.ColumnNumber; j++)
                                {
                                    raw.Add(ws.Cell(i, j).Value.ToString());
                                }
                                sheet.Add(raw);
                            }

                            xlsx.Add(sheet);
                        }
                    }
                }

                return new ExcelUploadResult
                {
                    RawExcel = xlsx,
                    SheetNames = sheetNames
                };
            }
        }

        /// <summary>
        /// 動的にdynamic型を生成する
        /// </summary>
        /// <param name="Fields">フィールド名とそのオブジェクト(このメソッドで生成したdynamicでも良い)の組み合わせ</param>
        /// <returns></returns>
        private dynamic InputDynamic(Dictionary<string, object> Fields)
        {
            dynamic result = new ExpandoObject();
            IDictionary<string, object> work = result;
            foreach (var item in Fields) { work.Add(item.Key, item.Value); }

            return result;
        }
    }

}