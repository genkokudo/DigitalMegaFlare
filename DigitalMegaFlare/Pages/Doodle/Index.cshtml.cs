using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Result Data { get; private set; }

        public string Test { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new Query { Id = 1 });
            return Page();
        }

        public IActionResult OnPost()
        {
            ViewData["Message"] = "ボタンを押しますたm9(･Å･)";
            return Page();
        }
    }

    /// <summary>検索条件</summary>
    public class Query : IRequest<Result>
    {
        public long Id { get; set; } 
    }

    /// <summary>検索結果</summary>
    public class Result
    {
        /// <summary>検索した情報</summary> 
        public TestData[] Datas { get; set; }
    }

    /// <summary> 
    /// 検索ハンドラ 
    /// QueryをSendすると動作し、Resultを返す 
    /// </summary> 
    public class QueryHandler : IRequestHandler<Query, Result>
    {
        private readonly ApplicationDbContext _db;

        public QueryHandler(ApplicationDbContext db)
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
        public async Task<Result> Handle(Query query, CancellationToken token)
        {
            // DB検索
            var datas = _db.TestDatas.AsNoTracking().ToArray(); 

            // 検索結果の格納
            var result = new Result
            {
                Datas = datas
            };
            return await Task.FromResult(result);
        }
    }
}