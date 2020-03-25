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

	// razorもDBで管理しましょう
	// 格納は階層構造にする。
	
	// ・階層構造の方法
	////// 階層構造って言うけど3階層固定なので、親子フィールド作る方法ではない（面倒だから）
	////// 3段階名前つけるだけ

	// 親子関係で良い気がする
	// 1:Parent == null
	// 2:Parent == Name && Razor == null
	// 3:Parent == Name && Razor != null

	// ・リストボックス選択肢
	// text:Name
	// value:Name（textと同じ）、ファイルの場合はID

	// ・ディレクトリ追加方法
	// ディレクトリ用テーブル作る・・・やっぱいらない
	// 普通にレコード追加するだけでいい。

	// ■ 優先度低い
	// ロック機能あり
	// 削除あり
}
