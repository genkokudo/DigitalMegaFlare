using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalMegaFlare.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace DigitalMegaFlare.Pages.SimpleGenerate.Razor
{
    // 保存されているファイルの一覧を表示する
    // プルダウン3つでやってみよう

    /// <summary>
    /// 保存されているファイルの一覧を表示する
    /// </summary>
    public class TemplateUploadModel : PageModel
    {
        private readonly IMediator _mediator;

        public TemplateUploadModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        public TemplateUploadResult Data { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new TemplateUploadQuery());
            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }

    /// <summary>検索条件</summary>
    public class TemplateUploadQuery : IRequest<TemplateUploadResult>
    {
        // なし
    }

    /// <summary>検索結果</summary>
    public class TemplateUploadResult
    {
        /// <summary>言語・ライブラリ一覧</summary> 
        public List<string> Libraries { get; set; }
        /// <summary>ディレクトリ一覧</summary>
        public Dictionary<string, List<string>> Directories { get; set; }
        /// <summary>ファイル一覧</summary>
        public Dictionary<string, List<string>> Files { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ
    /// TemplateUploadQueryをSendすると動作し、TemplateUploadResultを返す 
    /// </summary> 
    public class TemplateUploadQueryHandler : IRequestHandler<TemplateUploadQuery, TemplateUploadResult>
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public TemplateUploadQueryHandler(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// 検索の方法を定義する
        /// </summary>
        /// <param name="query">検索条件</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TemplateUploadResult> Handle(TemplateUploadQuery query, CancellationToken token)
        {
            // TODO:リストボックスのやり方忘れた

            // ファイル検索
            var libraries = new List<SelectListItem>();
            var directories = new Dictionary<string, List<SelectListItem>>();
            var files = new Dictionary<string, List<SelectListItem>>();

            // ↓これは駄目。ツリー構造のデータなのでツリークラスを使って処理。

            //// 1階層目
            //var razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");
            //IEnumerable<string> subFolders = Directory.GetDirectories(razorFileDirectry, "*", SearchOption.AllDirectories);

            //foreach (string subFolder in subFolders)
            //{
            //    libraries.Add(subFolder);
            //}

            //// 2階層目
            //foreach (var library in libraries)
            //{
            //    var subDirectories = new List<string>();
            //    var subDirectry = Path.Combine(razorFileDirectry, library);
            //    IEnumerable<string> subsubFolders = Directory.GetDirectories(subDirectry, "*", SearchOption.AllDirectories);

            //    foreach (string subsubFolder in subsubFolders)
            //    {
            //        subDirectories.Add(subsubFolder);
            //    }
            //    directories.Add(subDirectories);
            //}

            //// 3階層目
            //foreach (var library in libraries)
            //{
            //    var subDirectories = new List<string>();
            //    var subDirectry = Path.Combine(razorFileDirectry, library);
            //    IEnumerable<string> subsubFolders = Directory.GetFiles(subDirectry, "*", SearchOption.AllDirectories);

            //    foreach (string subsubFolder in subsubFolders)
            //    {
            //        subDirectories.Add(subsubFolder);
            //    }
            //    directories.Add(subDirectories);
            //}


            // 検索結果の格納
            var result = new TemplateUploadResult
            {
                Libraries = libraries,
                Directories = directories,
                Files = files
            };
            return await Task.FromResult(result);
        }

        private List<SelectListItem> getSelectList(int category)    // それぞれのValueって何入れよう？→下位を特定するためのキー。アンダースコアで繋げばとりあえずOK
        {
            // リストボックス選択肢の作成
            // 1階層目
            var razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");
            IEnumerable<string> subFolders = Directory.GetDirectories(razorFileDirectry, "*", SearchOption.AllDirectories);

            foreach (string subFolder in subFolders)
            {
                libraries.Add(subFolder);
            }

            //var parameters = _context.SystemParameters.Where(p => p.CategoryId == category).OrderBy(p => p.OrderNo).ToList();
            //var selectList = new List<SelectListItem>();
            //foreach (var item in parameters)
            //{
            //    selectList.Add(new SelectListItem(item.Display, item.CurrentValue));
            //}

            return selectList;
        }
    }


}