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
//using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using DigitalMegaFlare.Settings;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DigitalMegaFlare
{
    public class Startup
    {
        /// <summary>
        /// 環境変数を取得するのに使用
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// 設定ファイルの値取得
        /// </summary>
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // RazorEngineに日本語がエスケープされないようにする
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin,
                                             UnicodeRanges.CjkSymbolsandPunctuation,
                                             UnicodeRanges.Hiragana,
                                             UnicodeRanges.Katakana,
                                             UnicodeRanges.CjkUnifiedIdeographs));

            if (Environment.EnvironmentName == SystemConstants.EnvDevelopment)
            {
                // 開発系はappsettingsから接続文字列とパスワードを取得する。
                services.AddDbContext<ApplicationDbContext>(options =>
                    // SQLServerを使用する
                    options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.Connection))
                );
            }
            else
            {
                // 本番系は接続先をappsettingsから、パスワードを環境変数から取得する
                // SQLServerを使用する
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.Connection)));
                //options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.Connection) + "Password=" + Configuration.GetValue<string>(SystemConstants.DbPasswordEnv) + ";")
                //);
            }
            // デフォルトUI
            // UI画面を自作しない場合、この設定でデフォルトのRegisterページUIが設定される
            // ユーザ認証に使用するデータを指定
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            // SignalRを使用する
            services.AddSignalR();

            //構成情報から、DefaultParametersクラスへバインド
            services.Configure<DefaultParameters>(this.Configuration.GetSection(SystemConstants.DefaultParameters));

            // cshtml修正後、リロードですぐブラウザに反映する
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            //MediatRを使用する
            services.AddMediatR(typeof(Startup));

        }

        // ランタイムから呼ばれるメソッド 
        // HTTPリクエストパイプラインの設定に使用する
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "deny");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");

                await next.Invoke();
            });

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

                // Azureにしたので削除
                //// 最初無かったけどエラー出たので追加した
                //// アプリをサーバのサブディレクトリに配置する
                //app.UsePathBase(Configuration.GetValue<string>(SystemConstants.PathBase));
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
