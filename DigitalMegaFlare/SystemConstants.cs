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
        /// 非公開ファイルアップロード先
        /// </summary>
        public const string PrivateUploadsDirectry = "PrivateUploads";
        /// <summary>
        /// 公開ファイルアップロード先
        /// </summary>
        public const string PublicUploads = "PublicUploads";
        /// <summary>
        /// 公開ファイルアップロード先
        /// </summary>
        public const string PublicUploadsDirectry = "wwwroot/" + PublicUploads;

        /// <summary>
        /// アプリケーション設定：システムパラメータ初期値
        /// </summary>
        public const string DefaultParameters = "DefaultParameters";
        /// <summary>
        /// 初期値設定ユーザ名
        /// </summary>
        public const string DefaultParameterUserName = "default";
        /// <summary>
        /// システムパラメータ：分類コード：分類
        /// </summary>
        public const int SystemPatameterCategory = 1;
        /// <summary>
        /// システムパラメータ：分類コード：オンラインメモ
        /// </summary>
        public const int SystemPatameterMemo = 2;
        /// <summary>
        /// システムパラメータ：分類コード：メモ一覧の背景色
        /// </summary>
        public const int SystemPatameterMemoBack = 3;
        /// <summary>
        /// システムパラメータ：分類コード：システムモード切替
        /// </summary>
        public const int SystemPatameterMode = 4;
        /// <summary>
        /// システムパラメータ：分類コード：ゲストの投稿ロック
        /// </summary>
        public const int SystemPatameterModeDemo = 1;
        /// <summary>
        /// システムパラメータ：分類コード：システムモード切替、管理者権限登録可能
        /// </summary>
        public const int SystemPatameterModeRegisterAdmin = 2;
        /// <summary>
        /// ViewData：ゲストの投稿ロック
        /// </summary>
        public const string IsSubmitLocked = "IsSubmitLocked";
        /// <summary>
        /// ViewData：システムモード切替、管理者権限登録可能
        /// </summary>
        public const string IsRegisterAdmin = "IsRegisterAdmin";
    }
}
