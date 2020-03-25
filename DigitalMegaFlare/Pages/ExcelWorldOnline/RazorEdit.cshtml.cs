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
using Mintea.HtmlToDom;
using Mintea.Extensions;
using DigitalMegaFlare.Models;
using System.Text;

namespace DigitalMegaFlare.Pages.ExcelWorldOnline
{
    /// <summary>
    /// 保存されているファイルの一覧を表示する
    /// </summary>
    public class RazorEditModel : PageModel
    {
        #region リストボックス
        // リストボックスはformの名前指定でサーバに送れない？
        // 多分hiddenとjsを使えばできるが。
        // なのでBindPropertyする。

        #endregion
        /// <summary>
        /// 選択した言語・ライブラリ
        /// 未選択はnull
        /// </summary>
        [BindProperty]
        public string SelectMain { get; set; }

        /// <summary>
        /// 選択した分類
        /// 未選択はnull
        /// </summary>
        [BindProperty]
        public string SelectSub { get; set; }

        /// <summary>
        /// 選択したファイルID
        /// 未選択はnull
        /// </summary>
        [BindProperty]
        public string RazorId { get; set; }

        private readonly IMediator _mediator;

        public RazorEditModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 検索結果：ファイル階層構造など
        /// </summary>
        public GetFilesResult Data { get; private set; }

        /// <summary>
        /// 表示時の処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new GetFilesQuery());
            return Page();
        }

        /// <summary>
        /// 更新ボタンの処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync(string mainName, string subName, string fileName, string razorScript)
        {
            // Razorが空だったら拒否
            if (string.IsNullOrWhiteSpace(razorScript))
            {
                ViewData["Error"] = "ちょっとぐらいなにか書け。";
                return await OnGetAsync();
            }

            // メインを決定する
            var main = SelectMain;
            if (!string.IsNullOrWhiteSpace(mainName))
            {
                main = mainName;
            }
            if (string.IsNullOrWhiteSpace(main))
            {
                ViewData["Error"] = "メインの名前が分からん。";
                return await OnGetAsync();
            }

            // サブを決定する
            var sub = SelectSub;
            if (!string.IsNullOrWhiteSpace(subName))
            {
                sub = subName;
            }
            if (string.IsNullOrWhiteSpace(sub))
            {
                ViewData["Error"] = "サブの名前が分からん。";
                return await OnGetAsync();
            }

            // ファイルを決定する
            if (string.IsNullOrWhiteSpace(RazorId) && string.IsNullOrWhiteSpace(fileName))
            {
                ViewData["Error"] = "ファイル名が分からん。";
                return await OnGetAsync();
            }

            var result = await _mediator.Send(new UpdateQuery { 
                RazorId = RazorId,
                MainName = main,
                SubName = sub,
                FileName = fileName,
                RazorScript = razorScript
            });

            ViewData["Message"] = result.Result;
            return await OnGetAsync();
        }
    }

    #region GetFiles
    /// <summary>検索条件</summary>
    public class GetFilesQuery : IRequest<GetFilesResult>
    {
        // なし
    }

    /// <summary>検索結果</summary>
    public class GetFilesResult
    {
        /// <summary>ディレクトリ・ファイル一覧</summary>
        public Dictionary<string, List<SelectListItem>> Files { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ
    /// TemplateUploadQueryをSendすると動作し、TemplateUploadResultを返す 
    /// </summary> 
    public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, GetFilesResult>
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly ApplicationDbContext _db = null;
        public GetFilesQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 検索の方法を定義する
        /// </summary>
        /// <param name="query">検索条件</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GetFilesResult> Handle(GetFilesQuery query, CancellationToken token)
        {
            // リストボックスの選択肢を検索する
            var files = new Dictionary<string, List<SelectListItem>>
            {
                // rootのリストは空文字列がキー
                { string.Empty, new List<SelectListItem>() }
            };

            // 3階層しかないので、再帰的なことはしないで適当に検索する
            var roots = _db.RazorFiles.Where(x => x.Parent == null).ToList();
            foreach (var root in roots)
            {
                if (!files.ContainsKey(root.Name))
                {
                    // rootリストに追加
                    files[string.Empty].Add(new SelectListItem(root.Name, root.Name));

                    // subリスト追加
                    files.Add(root.Name, new List<SelectListItem>());
                }
                var subs = _db.RazorFiles.Where(x => x.Parent.Id == root.Id && x.Razor == null).ToList();
                foreach (var sub in subs)
                {
                    if (!files.ContainsKey(sub.Name))
                    {
                        // subリストに追加
                        files[root.Name].Add(new SelectListItem(sub.Name, sub.Name));

                        // ファイルリスト追加
                        files.Add(sub.Name, new List<SelectListItem>());
                    }
                    var razors = _db.RazorFiles.Where(x => x.Parent.Id == sub.Id && x.Razor != null).ToList();
                    foreach (var razor in razors)
                    {
                        // ファイル追加
                        files[sub.Name].Add(new SelectListItem(razor.Name, razor.Id.ToString()));
                    }
                }
            }

            // 検索結果の格納
            var result = new GetFilesResult
            {
                Files = files
            };
            return await Task.FromResult(result);
        }
    }
    #endregion


    /// <summary>検索条件</summary>
    public class UpdateQuery : IRequest<UpdateResult>
    {
        public string RazorId { get; set; }
        public string MainName { get; set; }
        public string SubName { get; set; }
        public string FileName { get; set; }
        public string RazorScript { get; set; }
    }

    /// <summary>検索結果</summary>
    public class UpdateResult
    {
        /// <summary>検索した情報</summary> 
        public string Result { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// UpdateQueryをSendすると動作し、UpdateResultを返す 
    /// </summary> 
    public class UpdateQueryHandler : IRequestHandler<UpdateQuery, UpdateResult>
    {
        private readonly ApplicationDbContext _db;

        public UpdateQueryHandler(ApplicationDbContext db)
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
        public async Task<UpdateResult> Handle(UpdateQuery query, CancellationToken token)
        {
            byte[] razor;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(query.RazorScript)))
            {
                razor = stream.ToArray();
            }

            // ファイルの保存
            if (!string.IsNullOrWhiteSpace(query.FileName))
            {
                // 入力があれば新規
                // 新規メイン
                var mainData = _db.RazorFiles.FirstOrDefault(x => x.Name == query.MainName && x.Parent == null);
                if (mainData == null)
                {
                    mainData = new RazorFile { Name = query.MainName, Parent = null, Razor = null };
                    _db.RazorFiles.Add(mainData);
                }
                // 新規サブ
                var subData = _db.RazorFiles.FirstOrDefault(x => x.Name == query.SubName && x.Parent == mainData);
                if (subData == null)
                {
                    subData = new RazorFile { Name = query.SubName, Parent = mainData, Razor = null };
                    _db.RazorFiles.Add(subData);
                }
                // 新規ファイル
                var fileData = new RazorFile { Name = query.FileName, Parent = subData, Razor = razor };
                _db.RazorFiles.Add(fileData);
            }
            else
            {
                // なければRazorIdを更新
                var data = _db.RazorFiles.First(x => x.Id == long.Parse(query.RazorId));
                data.Razor = razor;
                _db.RazorFiles.Update(data);
            }
            await _db.SaveChangesAsync();

            // 検索結果の格納
            var result = new UpdateResult
            {
                Result = "更新しました。"
            };
            return await Task.FromResult(result);
        }
    }

}