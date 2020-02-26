using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalMegaFlare.Entities
{
	/// <summary>
	/// サンプルモデルです。
	/// </summary>
	public class SampleEntity
	{
		/// <summary>
		/// IDを取得、もしくは、設定します。
		/// </summary>
		[Key]
		public long SampleId { get; set; }
		
		/// <summary>
		/// 名前A
		/// </summary>
		[StringLength(50)]
		public string NameA { get; set; }
		
		/// <summary>
		/// 点数B
		/// </summary>
		public int ScoreB { get; set; }

		/// <summary>
		/// 初期値を作成します。
		/// </summary>
		public static Sample InitialData
		{
			get
			{
				var names = new[] { "1", "2", "3", "4", "5" };
				var result = new List<SampleEntity>();
				foreach (var name in names)
				{
					result.Add(new SampleEntity() { Name = name });
				}
				return result.ToArray();
			}
		}
	}
}