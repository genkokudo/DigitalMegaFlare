using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace DigitalMegaFlare.Pages.SimpleGenerate.Razor
{
    public class CsvLaboModel : PageModel
    {
        public CsvLaboResult Data { get; private set; }

        private readonly IMediator _mediator = null;

        public CsvLaboModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new CsvLaboQuery { Id = 1 });
            if (Data.RawCsv == null)
            {
                ViewData["Error"] = "ファイルが存在しません。";
            }
            else if (Data.RawCsv.Count == 0)
            {
                ViewData["Error"] = "ファイルが空です";
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        } 
    }

    /// <summary>検索条件</summary>
    public class CsvLaboQuery : IRequest<CsvLaboResult>
    {
        public long Id { get; set; }
    }

    /// <summary>検索結果</summary>
    public class CsvLaboResult
    {
        /// <summary>CSVの内容</summary> 
        public List<string[]> RawCsv { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// QueryをSendすると動作し、Resultを返す 
    /// </summary> 
    public class CsvLaboQueryHandler : IRequestHandler<CsvLaboQuery, CsvLaboResult>
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public CsvLaboQueryHandler(IWebHostEnvironment hostEnvironment)
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
        public async Task<CsvLaboResult> Handle(CsvLaboQuery query, CancellationToken token)
        {

            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files");

            // ファイルの読み込み
            var csv = ReadCsv(fileDirectry);

            // 検索結果の格納
            var result = new CsvLaboResult
            {
                RawCsv = csv
            };
            return await Task.FromResult(result);
        }

        /// <summary>
        /// CSVを読み込む
        /// </summary>
        /// <param name="directry">ディレクトリ</param>
        /// <param name="filename">拡張子付きのファイル名</param>
        /// <returns></returns>
        private List<string[]> ReadCsv(string directry, string filename = "file.csv")
        {
            // ファイルの読み込み
            List<string[]> csv = null;
            using (PhysicalFileProvider provider = new PhysicalFileProvider(directry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo(filename);

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    csv = new List<string[]>();
                    // 改行コード、コンマで分けて格納する
                    var data = File.ReadAllText(fileInfo.PhysicalPath);
                    data = data.Replace("\r\n", "\n");
                    data = data.Replace("\r", "\n");
                    data = data.Trim('\n');
                    var lines = data.Split('\n');
                    foreach (var item in lines)
                    {
                        csv.Add(item.Split(','));
                    }
                }
                else
                {
                    return new List<string[]>();
                }
            }
            return csv;
        }
    }
}