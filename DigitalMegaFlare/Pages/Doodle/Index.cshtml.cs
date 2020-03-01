﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Mintea.RazorHelper;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
// "Doc"で始まるシートは読まない
// "List"で終わるシートは読む
// "Is"で始まるフィールドは、0,FALSE,False,-で始まる,空文字列はfalse、1,TRUE,Trueはtrue
// "Settings.Index"が必須。
// "RootList"の件数だけ生成する

// TODO:各出力ファイル名を指定できるようにすること。将来的には複数種類出力のため各行出力したいが。
// 現在は、RootListと、その添字をmodelに入れて送ってる感じ。Razor中でRootList[index]で生成している。
namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public IndexModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var engine = new RazorLightEngineBuilder()
                          .UseEmbeddedResourcesProject(typeof(Program))
                          .UseMemoryCachingProvider()
                          .DisableEncoding()
                          .Build();
            string outPath = Path.Combine(_hostEnvironment.WebRootPath, "temp");
            RazorHelper.SafeCreateDirectory(outPath);

            string dateFormat = "yyyyMMddHHmmss";
            string outFilePath = Path.Combine(outPath, $"{DateTime.UtcNow.ToString(dateFormat)}.zip");
            string excelName = "Model.xlsx";

            // 一時ファイル消す
            DirectoryInfo target = new DirectoryInfo(outPath);
            foreach (FileInfo file in target.GetFiles())
            {
                file.Delete();
            }

            // テンプレート読み込み
            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors", "asp");

            using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo("ModelCmp.dat");

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    // Razorスクリプト読み込み
                    var template = System.IO.File.ReadAllText(fileInfo.PhysicalPath);

                    // Excelから読み込み
                    var excelDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "excels");
                    var excel = RazorHelper.ReadExcel(excelDirectry, excelName, true);

                    // Modelの作成
                    dynamic model = null ;
                    try
                    {
                        model = RazorHelper.CreateModel(excel);
                    }
                    catch (Exception e)
                    {
                        ViewData["Error"] = e.Message;
                        return Page();
                    }

                    // ソース生成
                    string result = "";
                    var outFileList = new List<string>();
                    // ↓この"RootList"は動的に変えられないので、ファイル生成の一覧となるListシートの名前は"RootList"固定にする。
                    for (int i = 0; i < model.RootList.Count; i++)
                    {
                        // 変数入れる
                        model.Settings.Index = i.ToString();

                        // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
                        result = await engine.CompileRenderStringAsync($"{model.RootList[i].Name}", template, model);

                        // 生成したファイルを一時保存（今回はやっつけで。本当は人によって一時フォルダ名変えるべき。）
                        // VisualStudioが勘違いを起こすのでファイル末尾に"_"をつける
                        var outFileName = $"{model.RootList[i].Name}Entity.cs_";
                        outFileList.Add(outFileName);
                        System.IO.File.WriteAllText(Path.Combine(outPath, outFileName), result, System.Text.Encoding.UTF8);
                    }

                    // 一時保存したファイルをZipにする
                    using (ZipArchive archive = ZipFile.Open(outFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var item in outFileList)
                        {
                            archive.CreateEntryFromFile(
                                Path.Combine(outPath, $"{item}"),
                                $"{item.TrimEnd('_')}",
                                //$"{excelName}/item.TrimEnd('_')}", // ディレクトリ分けする場合はこう書く
                                CompressionLevel.NoCompression
                                );
                        }
                    }

                    ViewData["Message"] = result;
                }
                else
                {
                    ViewData["Error"] = "ファイルが存在しません。";
                }
            }

            return File(new FileStream(outFilePath, FileMode.Open), "application/zip", $"{excelName}.zip");

        }
    }
}
