using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalMegaFlare.Models
{
	/// <summary>
	/// テストモデル
	/// </summary>
	public class Test
	{
		/// <summary>
		/// 通し番号
		/// </summary>
		[Key]
		public long Id { get; set; }
		
		/// <summary>
		/// 名前
		/// </summary>
		[StringLength(100)]
		public string Name { get; set; }
		
		/// <summary>
		/// 点数
		/// </summary>
		public int Score { get; set; }
	}
}


