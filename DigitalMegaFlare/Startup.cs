using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using DigitalMegaFlare.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using DigitalMegaFlare.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMegaFlare
{
    public class Startup
    {
        //public const string RootsName = "default";
        //public const string RootsTemplate = "{controller=Home}/{action=Index}/{id?}";
        /// <summary>
        /// 環境変数を取得するのに使用
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// 設定ファイルの値取得
        /// </summary>
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            //構成ファイル、環境変数等から、構成情報をロード
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            //構成情報をプロパティに設定
            Configuration = builder.Build();    // IConfiguration configurationをインジェクションしない
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.EnvironmentName == SystemConstants.EnvDevelopment)
            {
                // 開発系はappsettingsから接続文字列とパスワードを取得する。
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString(SystemConstants.Connection),
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 3, 13), ServerType.MariaDb);
                    }
                ));
            }
            else
            {
                // 本番系は接続先をappsettingsから、パスワードを環境変数から取得する
                // マイグレーションを行う場合、環境名は"Development"になり、環境変数から値が取れないのでここは使えない。
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString(SystemConstants.Connection) + "Password=" + Configuration.GetValue<string>(SystemConstants.DbPasswordEnv) + ";",
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 3, 13), ServerType.MariaDb);
                    }
                ));
            }
            // デフォルトUI
            // UI画面を自作しない場合、この設定でデフォルトのRegisterページUIが設定される
            // ユーザ認証に使用するデータを指定
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            // SignalRを使用する
            services.AddSignalR();

            //// TODO:いるかどうか分からない。いらなかったらノートの「設定ファイルの読み込み」ページを直す。
            //// DIするのに必要？RazorPagesでは多分いらない
            //services.AddMvc();
            // RazorPagesを使用する設定
            // RazorPagesの設定なので、Pagesフォルダじゃないと適用されない。
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //構成情報から、DefaultParametersクラスへバインド
            services.Configure<DefaultParameters>(this.Configuration.GetSection(SystemConstants.DefaultParameters));

            // cshtml修正後、リロードですぐブラウザに反映する
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // ランタイムから呼ばれるメソッド 
        // HTTPリクエストパイプラインの設定に使用する
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // HSTS(HTTP Strict Transport Security Protocol)はデフォルト設定で30日
                app.UseHsts();

                //// 指定した URL でパイプラインを再実行するので UseRouting の前に呼ぶ必要がある
                //app.UseStatusCodePagesWithReExecute("/Error/Index", "?statusCode={0}");

                // 最初無かったけどエラー出たので追加した
                // アプリをサーバのサブディレクトリに配置する
                app.UsePathBase(Configuration.GetValue<string>(SystemConstants.PathBase));
            }

            // HTTPをHTTPSにリダイレクトする
            app.UseHttpsRedirection();

            // 静的ファイルのルーティング設定
            // UsePathBaseの後に書かなければならない
            // /wwwroot 配下のファイルに対して直接 URL アクセスが可能となる
            // /wwwroot/css/site.css というファイルに対しては http://..../css/site.css という URL でアクセスを行うことができる。
            // UseRoutingの前に書かなければならない
            app.UseStaticFiles();

            //app.Use((context, next) =>  // TODO:最初書いてなかったけど多分必要
            //{
            //    context.Request.Scheme = "https";
            //    return next();
            //});

            // ここでルートのマッチングが行われ、結果は HttpContext にセット
            app.UseRouting();

            // ユーザ認証を行う
            // 認証・認可はルーティングの結果が必要
            app.UseAuthentication();
            app.UseAuthorization();
            //// CORS もルーティングの結果が必要
            //app.UseCors();

            // cookieポリシーを使用する
            // これをUseMvc()より前に書くと、クライアントに提供するCookieが渡されないのでセッションが維持できない。
            app.UseCookiePolicy();

            // 2.2のEndpointRoutingと違う
            // パイプラインで明示的に UseRouting と UseEndpoints を呼び出す必要があります。
            // それぞれのメソッドはルートのマッチングを行うタイミングと、実際にリクエストの処理を行うタイミングを表しています。
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
                endpoints.MapRazorPages();
            });

        }
    }
}
