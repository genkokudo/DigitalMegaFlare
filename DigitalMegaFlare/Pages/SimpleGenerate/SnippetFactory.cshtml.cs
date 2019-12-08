using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalMegaFlare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mintea.SnippetGenerator;

namespace DigitalMegaFlare.Pages.SimpleGenerate
{
    public class SnippetFactoryModel : PageModel
    {
        [BindProperty]
        public Snippet Input { get; set; }

        [BindProperty]
        public string Output { get; set; }

        // TODO:クッキー
        private const string cKey = "TestKey";
        private string cValue = "TestValue";
        public IActionResult OnGetAsync()
        {
            // TODO:クッキーから読めないかなあー
            HttpContext.Response.Cookies.Append(cKey, cValue);

            // 画面項目を設定
            Input = new Snippet();
            return Page();
        }

        /// <summary>
        /// XML生成
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostGenerateXmlAsync()
        {
            var generator = new SnippetGenerator();
            ViewData["Output"] = generator.MakeSnippetXml(Input).ToString();
            return Page();
        }

        /// <summary>
        /// ダウンロード
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDownloadAsync()
        {
            var generator = new SnippetGenerator();
            Output = generator.MakeSnippetXml(Input).ToString();
            return Page();
        }
    }
}