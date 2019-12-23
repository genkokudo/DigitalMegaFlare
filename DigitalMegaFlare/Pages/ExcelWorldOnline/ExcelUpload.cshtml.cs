using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinteaCore.RazorHelper;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// TODO:OutはRootList、OutListはListとしてmodelに格納しないと互換性がなくなってしまう

namespace DigitalMegaFlare.Pages.ExcelWorldOnline
{
    public class ExcelUploadModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        private readonly ApplicationDbContext _db;

        private readonly IMediator _mediator = null;
        public ExcelUploadModel(IWebHostEnvironment hostEnvironment, IMediator mediator, ApplicationDbContext db)
        {
            _hostEnvironment = hostEnvironment;
            _mediator = mediator;
            _db = db;
        }
        public ExcelUploadResult Data { get; private set; }
        public ExcelListResult History { get; private set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if (Data == null)
            {
                Data = await _mediator.Send(new ExcelUploadQuery());
            }
            // 一覧
            History = await _mediator.Send(new ExcelListQuery());
            return Page();
        }

        #region ロック、ダウンロード、詳細ボタン
        /// <summary>
        /// ロックボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLockAsync(long id)
        {
            // DBに登録する
            var data = _db.ExcelFiles.First(x => x.Id == id);
            data.IsLocked = !data.IsLocked;
            _db.ExcelFiles.Update(data);
            await _db.SaveChangesAsync();

            // 再検索
            return await OnGetAsync();
        }

        /// <summary>
        /// ダウンロードボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult OnPostDownload(long id)
        {
            var data = _db.ExcelFiles.First(x => x.Id == id);
            return File(data.Xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", data.RawFileName);
        }

        /// <summary>
        /// 詳細ボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDetailAsync(long id)
        {
            Data = await _mediator.Send(new ExcelUploadQuery { Id = id });
            return await OnGetAsync();
        }
        #endregion

        #region 削除ボタン
        /// <summary>
        /// 削除ボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            // DBからレコードを削除
            var data = _db.ExcelFiles.First(x => x.Id == id);
            _db.ExcelFiles.Remove(data);
            await _db.SaveChangesAsync();

            // 再検索
            return await OnGetAsync();
        }
        #endregion

        #region 生成ボタン

        #region 外部生成
        /// <summary>
        /// ソースを生成する
        /// 外部用
        /// </summary>
        /// <param name="excel">Excel</param>
        /// <param name="engine">エンジン</param>
        /// <returns>[Name][生成結果]：Outが無かったらNull</returns>
        private async Task<Dictionary<string, string>> GenerateOut(Dictionary<string, List<List<string>>> excel, RazorLightEngine engine)
        {
            if (excel.ContainsKey("Out"))
            {
                // Outの入力を作成する
                dynamic model;

                model = RazorHelper.CreateOutModel(excel);

                var generatedSource = new Dictionary<string, string>();
                for (int i = 0; i < model.RootList.Count; i++)
                {
                    // 変数入れる
                    model.General.Index = i.ToString();

                    // テンプレート読み込み
                    var template = GetTemplate((string)model.RootList[i].RazorTemplate);

                    // ソース生成
                    // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
                    generatedSource.Add((string)model.RootList[i].Name, await engine.CompileRenderStringAsync($"{model.RootList[i].Name}Out", template, model));
                }

                // Outが無くても空ディクショナリを返す
                return generatedSource;
            }
            return null;
        }

        #region GetTemplate
        /// <summary>
        /// DBからテンプレート取得
        /// </summary>
        /// <param name="path">cs_asp/crud/ListBox</param>
        /// <returns></returns>
        private string GetTemplate(string path)
        {
            // メイン、サブ、ファイルの3つの名前でアクセス
            var splitedPath = path.Trim('/').Split("/");
            var razorData = _db.RazorFiles.First(x => x.Name == splitedPath[2] && x.Parent.Name == splitedPath[1] && x.Parent.Parent.Name == splitedPath[0]);
            var template = string.Empty;
            using (var stream = new MemoryStream(razorData.Razor))
            {
                template = Encoding.UTF8.GetString(stream.ToArray());
            }
            return template;
        }

        #endregion

        #endregion

        /// <summary>
        /// 生成ボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostGenerateAsync(long id)
        {
            try
            {
                // エンジンを生成
                var engine = new RazorLightEngineBuilder()
                          .UseEmbeddedResourcesProject(typeof(Program))
                          .UseMemoryCachingProvider()
                          .DisableEncoding()
                          .Build();

                // 対象のExcelを読み込む
                var data = _db.ExcelFiles.First(x => x.Id == id);

                Dictionary<string, List<List<string>>> excel = null;
                using (var stream = new MemoryStream(data.Xlsx))
                {
                    excel = RazorHelper.ReadExcel(stream, true);
                }

                // 外部生成
                var outInput = await GenerateOut(excel, engine);

                // Modelの作成
                dynamic model;
                model = RazorHelper.CreateModel(excel, outInput);

                // 一時出力先を作成
                string outPath = Path.Combine(_hostEnvironment.WebRootPath, "temp");
                RazorHelper.DeleteDirectory(outPath, true); // linuxだとパーミッションが消えてダウンロードできなくなるので最初のフォルダは残す
                RazorHelper.SafeCreateDirectory(outPath);
                // 一時ファイル消す
                RazorHelper.DeleteFiles(outPath);

                // リストを読んでソース生成する
                // ↓この"RootList"は動的に変えられないので、ファイル生成の一覧となるListシートの名前は"RootList"固定にする。
                var outFileList = new List<string>();
                var razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");

                // 各ソース生成
                List<dynamic> generatedSource = await GenerateSource(model, engine);

                for (int i = 0; i < model.RootList.Count; i++)
                {
                    // ファイル名生成
                    var resultFilename = await engine.CompileRenderStringAsync(
                        $"{model.RootList[i].Name}Name",
                        model.RootList[i].OutputFileName,
                        new
                        {
                            model.RootList[i].Name,
                            model.RootList[i].Camel,
                            model.RootList[i].Pascal,
                            model.RootList[i].Plural,
                            model.RootList[i].Hyphen,
                            model.RootList[i].Snake,
                            model.RootList[i].CamelPlural,
                            model.RootList[i].PascalPlural
                        });

                    // 生成したファイルを一時保存
                    // VisualStudioが勘違いを起こすのでファイル末尾に"_"をつける
                    var outFileName = $"{resultFilename}_";
                    outFileList.Add(outFileName);

                    // ディレクトリ分けしたZipを作成する
                    RazorHelper.SafeCreateDirectory(Path.Combine(outPath, Path.GetDirectoryName(outFileName)));
                    System.IO.File.WriteAllText(Path.Combine(outPath, outFileName), generatedSource[i], Encoding.UTF8);
                }

                // 圧縮ファイルの準備
                string dateFormat = "yyyyMMddHHmmss";
                string outFilePath = Path.Combine(outPath, $"{DateTime.UtcNow.ToString(dateFormat)}.zip");
                // 一時保存したファイルをZipにする
                using (ZipArchive archive = ZipFile.Open(outFilePath, ZipArchiveMode.Create))
                {
                    foreach (var item in outFileList)
                    {
                        archive.CreateEntryFromFile(
                            Path.Combine(outPath, $"{item}"),
                            $"{item.TrimEnd('_')}", // ここでスラッシュを入れると、ディレクトリ分けしたZipが作成できる
                            CompressionLevel.NoCompression
                        );
                    }
                }

                return File(new FileStream(outFilePath, FileMode.Open), "application/zip", $"{data.RawFileName}.zip");
            }
            catch (Exception e)
            {
                ViewData["Error"] = e.Message;
                return Page();
            }
        }

        /// <summary>
        /// ソースを生成する
        /// </summary>
        /// <param name="model">モデル</param>
        /// <param name="engine">エンジン</param>
        /// <returns></returns>
        private async Task<List<dynamic>> GenerateSource(dynamic model, RazorLightEngine engine)
        {
            var generatedSource = new List<dynamic>();
            for (int i = 0; i < model.RootList.Count; i++)
            {
                // 変数入れる
                model.General.Index = i.ToString();

                // テンプレート読み込み
                var template = GetTemplate((string)model.RootList[i].RazorTemplate);

                // ソース生成
                // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
                var name = $"{model.RootList[i].Name}";
                generatedSource.Add(await engine.CompileRenderStringAsync(name, template, model));
            }

            return generatedSource;
        }
        #endregion

        #region Uploadボタン
        /// <summary>
        /// Excelアップロードボタン
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUploadAsync(IFormFile file, string comment)
        {
            // アップロードされたファイルをサーバに保存する
            using (var fileStream = file.OpenReadStream())
            {
                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
                var hostName = System.Net.Dns.GetHostEntry(ipAddress).HostName;
                var url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);

                // DBに保存する
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var xlsx = memoryStream.ToArray();

                    // DBに登録する
                    var data = new ExcelFile {
                        RawFileName = file.FileName,
                        Comment = comment,
                        Ip = ipAddress.ToString(),
                        Host = hostName,
                        InputDate = DateTime.UtcNow,
                        IsLocked = false,
                        Url = url,
                        Xlsx = xlsx
                    };
                    _db.ExcelFiles.Add(data);
                    await _db.SaveChangesAsync();
                }
            }

            return await OnGetAsync();
        }

        /// <summary>
        /// 8桁のランダムで重複のない文字列を取得する
        /// </summary>
        /// <returns></returns>
        private string GetUniqueName()
        {
            return Path.GetRandomFileName().Split(".")[0];
        }
        #endregion
    }

    #region History

    /// <summary>検索条件</summary>
    public class ExcelListQuery : IRequest<ExcelListResult>
    {
        // 何もなし
    }

    /// <summary>検索結果</summary>
    public class ExcelListResult
    {
        /// <summary>検索した情報</summary> 
        public ExcelFile[] Histories { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// QueryをSendすると動作し、Resultを返す 
    /// </summary> 
    public class ExcelListQueryHandler : IRequestHandler<ExcelListQuery, ExcelListResult>
    {
        private readonly ApplicationDbContext _db;

        public ExcelListQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 検索の方法を定義する
        /// IRequestHandlerで実装することになっている
        /// </summary>
        /// <param name="query">検索条件</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ExcelListResult> Handle(ExcelListQuery query, CancellationToken token)
        {
            // DB検索
            var histories = _db.ExcelFiles.ToArray();

            // 検索結果の格納
            var result = new ExcelListResult
            {
                Histories = histories
            };
            return await Task.FromResult(result);
        }
    }
    #endregion

    #region Upload
    /// <summary>検索条件</summary>
    public class ExcelUploadQuery : IRequest<ExcelUploadResult>
    {
        /// <summary>ファイルパス</summary> 
        public long Id { get; set; }
    }

    /// <summary>検索結果</summary>
    public class ExcelUploadResult
    {
        /// <summary>シート名</summary> 
        public List<string> SheetNames { get; set; } = new List<string>();

        /// <summary>Excelの内容</summary> 
        public List<List<List<string>>> RawExcel { get; set; } = new List<List<List<string>>>();
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// QueryをSendすると動作し、Resultを返す 
    /// </summary> 
    public class ExcelUploadQueryHandler : IRequestHandler<ExcelUploadQuery, ExcelUploadResult>
    {
        private readonly ApplicationDbContext _db;
        public ExcelUploadQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 検索の方法を定義する
        /// IRequestHandlerで実装することになっている
        /// </summary>
        /// <param name="query">検索条件</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ExcelUploadResult> Handle(ExcelUploadQuery query, CancellationToken token)
        {
            if (query.Id == 0)
            {
                return new ExcelUploadResult();
            }

            // ファイルの読み込み
            // 検索結果の格納
            var data = _db.ExcelFiles.FirstOrDefault(x => x.Id == query.Id);
            using (var memoryStream = new MemoryStream(data.Xlsx))
            {
                var result = ReadExcel(memoryStream);
                return await Task.FromResult(result);
            }
        }

        /// <summary>
        /// Excelを読み込む
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private ExcelUploadResult ReadExcel(Stream stream)
        {
            // ファイルの読み込み
            List<string> sheetNames = new List<string>();
            List<List<List<string>>> xlsx = new List<List<List<string>>>();

            var excel = RazorHelper.ReadExcel(stream, false);

            foreach (var sheet in excel.Values)
            {
                xlsx.Add(sheet);
            }
            foreach (var sheetName in excel.Keys)
            {
                sheetNames.Add(sheetName);
            }

            return new ExcelUploadResult
            {
                RawExcel = xlsx,
                SheetNames = sheetNames
            };
        }

    }
    #endregion

}