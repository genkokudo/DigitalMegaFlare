using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DigitalMegaFlare.Pages.SimpleGenerate.Razor
{
    // やることリスト
    // Uploadボタンの確認
    // 一覧表示
    // ・マスター化
    // ・削除(bkfilesに移動)
    // ・生成
    // ・Excelダウンロード
    // ・Excelのプレビュー

    // 上が終わったら
    // Razorのディレクトリ作成機能付けようか。
    // これで完成。終わり。リリース。ReactへGO

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
            // Excelプレビュー
            Data = await _mediator.Send(new ExcelUploadQuery());

            // 一覧
            History = await _mediator.Send(new ExcelListQuery());

            if (Data.RawExcel == null)
            {
                ViewData["Error"] = "ファイルが存在しません。";
            }
            else if (Data.RawExcel.Count == 0)
            {
                ViewData["Error"] = "ファイルが空です";
            }
            return Page();
        }

        /// <summary>
        /// ロックボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLockAsync(long id)
        {
            // DBに登録する
            var data = _db.ExcelInputHistories.First(x => x.Id == id);
            data.IsLocked = !data.IsLocked;
            _db.ExcelInputHistories.Update(data);
            await _db.SaveChangesAsync();

            // 再検索
            return await OnGetAsync();
        }

        /// <summary>
        /// ダウンロードボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDownloadAsync(long id)
        {
            var data = _db.ExcelInputHistories.First(x => x.Id == id);
            //return File(System.Text.Encoding.UTF8.GetBytes(output), "application/xml", Input.Title + ".snippet");
            return await OnGetAsync();
        }

        /// <summary>
        /// 詳細ボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDetailAsync(long id)
        {
            var data = _db.ExcelInputHistories.First(x => x.Id == id);
            // 再検索
            return await OnGetAsync();
        }

        /// <summary>
        /// 削除ボタン
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var data = _db.ExcelInputHistories.First(x => x.Id == id);
            // 再検索
            return await OnGetAsync();
        }

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

                var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, SystemConstants.UploadedExcelsDirectory);

                using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
                {
                    // ファイル情報を取得
                    var serverFileName = GetUniqueName();
                    IFileInfo fileInfo = provider.GetFileInfo(serverFileName);   // ファイル情報

                    // 指定したパスに保存する
                    using (var stream = new FileStream(fileInfo.PhysicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                        // DBに登録する
                        var data = new ExcelInputHistory {
                            RawFileName = file.FileName,
                            Comment = comment,
                            FileName = serverFileName,
                            Ip = ipAddress.ToString(),
                            Host = hostName,
                            InputDate = DateTime.UtcNow,
                            IsLocked = false,
                            Url = url
                        };
                        _db.ExcelInputHistories.Add(data);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            // 再検索
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
        public ExcelInputHistory[] Histories { get; set; }
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
            var histories = _db.ExcelInputHistories.ToArray();

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
        // 何もなし
    }

    /// <summary>検索結果</summary>
    public class ExcelUploadResult
    {
        /// <summary>シート名</summary> 
        public List<string> SheetNames { get; set; }

        /// <summary>Excelの内容</summary> 
        public List<List<List<string>>> RawExcel { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// QueryをSendすると動作し、Resultを返す 
    /// </summary> 
    public class ExcelUploadQueryHandler : IRequestHandler<ExcelUploadQuery, ExcelUploadResult>
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public ExcelUploadQueryHandler(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
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
            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, SystemConstants.UploadedExcelsDirectory);

            // ファイルの読み込み
            // 検索結果の格納
            var result = ReadExcel(fileDirectry);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Excelを読み込む
        /// </summary>
        /// <param name="directry">ディレクトリ</param>
        /// <param name="filename">拡張子付きのファイル名</param>
        /// <returns></returns>
        private ExcelUploadResult ReadExcel(string directry, string filename = "file.xlsx")
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

                            //"行数:" + ws.LastCellUsed().Address.RowNumber.ToString()
                            //"列数:" + ws.LastCellUsed().Address.ColumnNumber.ToString()
                            //"列記号:" + ws.LastCellUsed().Address.ColumnLetter.ToString()

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

        #region Excelファイル作成（使ってない）
        ///// <summary>
        ///// Excelファイル作成
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //private async Task<XLWorkbook> BuildExcelFile(int id)
        //{
        //    var t = Task.Run(() =>
        //    {
        //        // ブック作成
        //        var wb = new XLWorkbook();
        //        // シート作成
        //        var ws = wb.AddWorksheet("Sheet1");
        //        // 最初のセルに値を設定
        //        ws.FirstCell().SetValue(id);
        //        // 保存
        //        //wb.SaveAs("HelloWorld.xlsx");
        //        return wb;
        //    });
        //    return await t;
        //}

        #endregion

    }
    #endregion

}