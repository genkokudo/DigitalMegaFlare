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
using MinteaCore.HtmlToDom;
using MinteaCore.Extensions;

namespace DigitalMegaFlare.Pages.ExcelWorldOnline.Botsu
{
    /// <summary>
    /// 保存されているファイルの一覧を表示する
    /// </summary>
    public class TemplateUploadModel : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public string FilePath { get; set; }

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
    }

    /// <summary>検索条件</summary>
    public class TemplateUploadQuery : IRequest<TemplateUploadResult>
    {
        // なし
    }

    /// <summary>検索結果</summary>
    public class TemplateUploadResult
    {
        /// <summary>ディレクトリ・ファイル一覧</summary>
        public Dictionary<string, List<SelectListItem>> Files { get; set; }
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
            var razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");

            // TODO:Blob化すること
            // リストボックスの選択肢を検索する
            var list = TreeNode<string>.GetDirectoryFileList(razorFileDirectry);

            var files = new Dictionary<string, List<SelectListItem>>();
            foreach (var file in list)
            {
                files.Add(file.Key, new List<SelectListItem>());
                foreach (var select in file.Value)
                {
                    files[file.Key].Add(new SelectListItem(select.Key, select.Value));
                }
            }

            // 検索結果の格納
            var result = new TemplateUploadResult
            {
                Files = files
            };
            return await Task.FromResult(result);
        }
    }


}