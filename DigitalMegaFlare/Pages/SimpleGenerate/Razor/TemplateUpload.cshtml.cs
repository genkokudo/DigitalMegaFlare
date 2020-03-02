using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalMegaFlare.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public List<List<string>> Directories { get; set; }
        /// <summary>ファイル一覧</summary>
        public List<List<List<string>>> Files { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ
    /// TemplateUploadQueryをSendすると動作し、TemplateUploadResultを返す 
    /// </summary> 
    public class TemplateUploadQueryHandler : IRequestHandler<TemplateUploadQuery, TemplateUploadResult>
    {
        /// <summary>
        /// 検索の方法を定義する
        /// </summary>
        /// <param name="query">検索条件</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TemplateUploadResult> Handle(TemplateUploadQuery query, CancellationToken token)
        {
            // ファイル検索
            //var datas = _db.TestDatas.AsNoTracking().ToArray();

            var libraries = new List<string> { "----- 言語・ライブラリ -----" };
            var directories = new List<List<string>> { "----- ディレクトリ -----" };
            var files = new List<List<List<string>>> { "----- ファイル -----" };

            // 検索結果の格納
            var result = new TemplateUploadResult
            {
                Libraries = libraries,
                Directories = directories,
                Files = files
            };
            return await Task.FromResult(result);
        }
    }

}