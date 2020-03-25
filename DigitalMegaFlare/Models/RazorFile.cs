using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalMegaFlare.Models
{
	/// <summary>
	/// Razorファイル管理
	/// </summary>
    public class RazorFile
	{
		/// <summary>
		/// IDを取得、もしくは、設定します。
		/// </summary>
		[Key]
		public long Id { get; set; }

		/// <summary>
		/// 親ディレクトリを取得、もしくは、設定します。
		/// </summary>
		public RazorFile Parent { get; set; }

		/// <summary>
		/// 名称を取得、もしくは、設定します。
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Razorのバイナリデータを取得、もしくは、設定します。
		/// </summary>
		public byte[] Razor { get; set; }
	}
}
