using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMegaFlare.Controllers
{
    /// <summary>
    /// Razor実験室のコントローラーです。
    /// </summary>
    //[Authorize]
    public class RazorController : Controller
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        private readonly string _backupFileDirectry = null;
        private readonly string _razorFileDirectry = null;
        public RazorController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _backupFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.BackupFileDirectory, "razors");
            _razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public ActionResult Load(string filepath)
        {
            var data = System.IO.File.ReadAllText(filepath);
            return Ok(new { Message = $"{data}" });
        }

        /// <summary>
        /// ファイル作成
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="subDir"></param>
        /// <param name="filename"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ActionResult Create(string mainDir, string subDir, string filename, string text)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return Ok(new { Message = "ファイル名書いて。" });
            }
            var filepath = Path.Combine(_razorFileDirectry, mainDir, subDir, filename + ".dat");
            if (System.IO.File.Exists(filepath))
            {
                // ファイルがあったら駄目
                return Ok(new { Message = "既に存在するファイル名なので保存しませんでした。"});
            }
            try
            {
                System.IO.File.WriteAllText(filepath, text);
            }
            catch (Exception)
            {
                return Ok(new { Message = "既に存在するファイル名なので保存しませんでした。" });
            }
            return Ok(new { Message = "保存しました。" });
        }

        /// <summary>
        /// ファイル更新
        /// バックアップ作成
        /// </summary>
        /// <param name="mainDir"></param>
        /// <param name="subDir"></param>
        /// <param name="filename"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ActionResult Update(string mainDir, string subDir, string filename, string text)
        {
            // 本物を取得
            var filepath = Path.Combine(_razorFileDirectry, mainDir, subDir, filename);
            SafeCreateDirectory(filepath);
            var data = System.IO.File.ReadAllText(filepath);

            // バックアップを取ろう
            var bkDir = Path.Combine(_backupFileDirectry, mainDir, subDir);
            SafeCreateDirectory(bkDir);

            string dateFormat = "yyyyMMddHHmmss";
            var now = DateTime.Now.ToString(dateFormat);

            System.IO.File.WriteAllText(Path.Combine(bkDir, filename + "_" + now), data);

            // 本物を保存
            System.IO.File.WriteAllText(Path.Combine(filepath), text);

            return Ok();
        }


        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合
        /// すべてのディレクトリとサブディレクトリを作成します
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static DirectoryInfo SafeCreateDirectory(string directory)
        {
            if (!directory.EndsWith("\\") && !directory.EndsWith("/"))
            {
                directory += "/";
            }
            if (Directory.Exists(directory))    // 環境によっては正しく判定出来ないらしい
            {
                return null;
            }
            try
            {
                return Directory.CreateDirectory(directory);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}