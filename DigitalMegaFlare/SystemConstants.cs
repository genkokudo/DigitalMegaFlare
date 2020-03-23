using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalMegaFlare
{
    /// <summary>
    /// システム設定
    /// </summary>
    public class SystemConstants
    {
        public const string FileDirectory = "files";
        public const string BackupFileDirectory = "bkfiles";

        /// <summary>
        /// Excelアップロード先
        /// </summary>
        public const string UploadedExcelsDirectory = "uploadedExcels";

        // TODO:使ってない定数沢山

        // 環境
        /// <summary>
        /// 環境：開発系
        /// </summary>
        public const string EnvDevelopment = "Development";
        /// <summary>
        /// 環境：本番系
        /// </summary>
        public const string EnvProduction = "Production";

        // 設定名
        /// <summary>
        /// 現在のDB接続設定
        /// 詳細はappsettings.json
        /// </summary>
        public const string Connection = "DefaultConnection";
        /// <summary>
        /// サブディレクトリに配置するときのパス
        /// </summary>
        public const string PathBase = "PathBase";
        /// <summary>
        /// 使用するポート番号
        /// </summary>
        public const string Port = "Port";

        // 環境変数名
        /// <summary>
        /// DBパスワード環境変数名
        /// </summary>
        public const string DbPasswordEnv = "DATABASE_PASSWORD";

        /// <summary>
        /// ページタイトル
        /// </summary>
        public const string Title = "DigitalMegaFlare";

        /// <summary>
        /// 管理者権限名
        /// </summary>
        public const string Administrator = "Administrator";
        /// <summary>
        /// ゲスト権限名
        /// </summary>
        public const string Guest = "Guest";

        /// <summary>
        /// ファイル未選択
        /// </summary>
        public const string NoFileError = "ファイルが選択されていません";

        /// <summary>
        /// パスワードの確認誤りメッセージ
        /// </summary>
        public const string ConfirmPasswordError = "パスワードが一致していません";

        /// <summary>
        /// コメント未入力
        /// </summary>
        public const string NoComment = "コメントなし";

        /// <summary>
        /// アプリケーション設定：システムパラメータ初期値
        /// </summary>
        public const string DefaultParameters = "DefaultParameters";
        /// <summary>
        /// 初期値設定ユーザ名
        /// </summary>
        public const string DefaultParameterUserName = "default";
    }
}
