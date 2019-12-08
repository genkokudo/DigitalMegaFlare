using Mintea.SnippetGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalMegaFlare.Models
{
    public class Snippet : SnippetData
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// デミリタ配列が画面で削除されたかのフラグ
        /// </summary>
        public bool[] DelimiterDeleteFlags { get; set; }

        public Snippet()
        {
        }

        /// <summary>
        /// 可変長要素以外で必須のものを設定する
        /// </summary>
        /// <param name="title">ファイル名</param>
        /// <param name="author">作者</param>
        /// <param name="description">説明</param>
        /// <param name="shortcut">ショートカットになるフレーズ</param>
        /// <param name="code">コード</param>
        /// <param name="language">言語</param>
        /// <param name="delimiter">特殊文字</param>
        /// <param name="kind">スニペットの種類</param>
        public Snippet(string title, string author, string description, string shortcut, string code, Language language, string delimiter, Kind kind)
            : base(title, author, description, shortcut, code, language, delimiter, kind)
        {
        }
    }
}
