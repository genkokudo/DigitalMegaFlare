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

namespace DigitalMegaFlare
{
    public class ExcelListModel : PageModel
    {

        private readonly IMediator _mediator;

        public ExcelListModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        public ExcelListResult Data { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Data = await _mediator.Send(new ExcelListQuery { Id = 1 });
            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }

    /// <summary>検索条件</summary>
    public class ExcelListQuery : IRequest<ExcelListResult>
    {
        public long Id { get; set; }
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
}

