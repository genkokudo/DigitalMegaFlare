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
using RazorLight;

namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public string Test { get; set; }

        public IActionResult OnGet()
        {
            //Data = await _mediator.Send(new Query { Id = 1 });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var engine = new RazorLightEngineBuilder()
                          // required to have a default RazorLightProject type, but not required to create a template from string.
                          .UseEmbeddedResourcesProject(typeof(Program))
                          .UseMemoryCachingProvider()
                          .Build();

            var template = "Name: @Model.Name, Age: @Model.Age";
            var model = new { Name = "hauhau", Age = 18 };

            string result = await engine.CompileRenderStringAsync("templateKey", template, model);

            ViewData["Message"] = result;
            return Page();

//            ■匿名型の動的作成 
            //dynamic tmp2 = new ExpandoObject(); 
            //var dic = new Dictionary<string, object>() { { "name", "hogehoge" }, { "age", 10 } }; 
 
            //IDictionary<string, object> wk = tmp2; 
            //foreach (var item in dic) { wk.Add(item.Key, item.Value); } 
 
            //Console.WriteLine("----- tmp2 -----"); 
            //Console.WriteLine(tmp2.name); 
            //Console.WriteLine(tmp2.age); 
        }
    }

}