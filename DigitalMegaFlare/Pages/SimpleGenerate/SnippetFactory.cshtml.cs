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
            SetOutput();
            return Page();
        }

        /// <summary>
        /// ダウンロード
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDownloadAsync()
        {
            string output = SetOutput();
            return File(System.Text.Encoding.UTF8.GetBytes(output), "application/xml", Input.Title + ".snippet");
        }

        private string SetOutput()
        {
            // Input.Declaration.Idが""だったらスルー
            Input.Declarations.RemoveAll(x => string.IsNullOrWhiteSpace(x.Id));

            // Input.Importsが""だったらスルー
            Input.Imports.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            var generator = new SnippetGenerator();
            var xml = generator.MakeSnippetXml(Input);
            var output = xml.ToString();
            ViewData["Output"] = output;
            return output;
        }
    }
}