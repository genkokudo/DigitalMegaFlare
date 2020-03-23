using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalMegaFlare.Models
{
	/// <summary>
	/// Excel読み込み履歴です。
	/// </summary>
	public class ExcelInputHistory
	{
		/// <summary>
		/// IDを取得、もしくは、設定します。
		/// </summary>
		[Key]
		public long Id { get; set; }

		/// <summary>
		/// ロック状態を取得、もしくは、設定します。
		/// </summary>
		public bool IsLocked { get; set; }

		/// <summary>
		/// 元ファイル名を取得、もしくは、設定します。
		/// </summary>
		public string RawFileName { get; set; }

		/// <summary>
		/// コメントを取得、もしくは、設定します。
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 取込日時を取得、もしくは、設定します。
		/// </summary>
		public DateTime InputDate { get; set; }

		/// <summary>
		/// アップロード者のリモートホスト情報を取得、もしくは、設定します。
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// アップロード者のIPアドレスを取得、もしくは、設定します。
		/// </summary>
		public string Ip { get; set; }

		/// <summary>
		/// アップロード者のリクエスト時URLを取得、もしくは、設定します。
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Excelのバイナリデータを取得、もしくは、設定します。
		/// </summary>
		public byte[] Xlsx { get; set; }

		public override string ToString()
		{
			return $"ID：{Id}, 取込日時：{InputDate}, ホスト：{Host}, IPアドレス：{Ip}, リクエスト時URL：{Url}";
		}
	}
}
