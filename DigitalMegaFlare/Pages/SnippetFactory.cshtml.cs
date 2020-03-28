using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalMegaFlare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinteaCore.SnippetGenerator;

namespace DigitalMegaFlare.Pages
{
    public class SnippetFactoryModel : PageModel
    {
        [BindProperty]
        public Snippet Input { get; set; }

        [BindProperty]
        public string Output { get; set; }

        // TODO:クッキー
        private const string CAuthor = "Author";
        public IActionResult OnGetAsync()
        {
            // 画面項目を設定
            Input = new Snippet();

            // クッキーから読む
            var author = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == CAuthor);
            if (author.Key != null)
            {
                Input.Author = author.Value;
            }

            return Page();
        }

        /// <summary>
        /// XML生成
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostGenerateXmlAsync()
        {
            // クッキーに書く
            HttpContext.Response.Cookies.Append(CAuthor, Input.Author);
            SetOutput();
            return Page();
        }

        /// <summary>
        /// ダウンロード
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDownloadAsync()
        {
            // クッキーに書く
            HttpContext.Request.Cookies.Append(new KeyValuePair<string, string>(CAuthor, Input.Author));
            string output = SetOutput();
            return File(System.Text.Encoding.UTF8.GetBytes(output), "application/xml", Input.Title + ".snippet");
        }

        private string SetOutput()
        {
            // Input.Declaration.Idが""だったらスルー
            Input.Declarations?.RemoveAll(x => string.IsNullOrWhiteSpace(x.Id));

            // Input.Importsが""だったらスルー
            Input.Imports?.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            var generator = new SnippetGenerator();
            var xml = generator.MakeSnippetXml(Input);
            var output = xml.ToString();
            ViewData["Output"] = output;
            return output;
        }
    }
}