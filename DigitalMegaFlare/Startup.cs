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
            // ユーザ認証に使用するデータを指定
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddSignalR();      // SignalRを使用する

            //// TODO:いるかどうか分からない。いらなかったらノートの「設定ファイルの読み込み」ページを直す。
            //// DIするのに必要？RazorPagesでは多分いらない
            //services.AddMvc();
            // RazorPagesを使用する設定
            // RazorPagesの設定なので、Pagesフォルダじゃないと適用されない。
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //構成情報から、DefaultParametersクラスへバインド
            services.Configure<DefaultParameters>(this.Configuration.GetSection(SystemConstants.DefaultParameters));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                //// アプリをサーバのサブディレクトリに配置する
                //app.UsePathBase(Configuration.GetValue<string>(SystemConstants.PathBase));
            }

            // HTTPをHTTPSにリダイレクトする
            app.UseHttpsRedirection();

            // 静的ファイルのルーティング設定
            // UsePathBaseの後に書かなければならない
            // /wwwroot 配下のファイルに対して直接 URL アクセスが可能となる
            // /wwwroot/css/site.css というファイルに対しては http://..../css/site.css という URL でアクセスを行うことができる。
            app.UseStaticFiles();

            app.UseRouting();

            // ユーザ認証を行う
            app.UseAuthentication();
            app.UseAuthorization();

            // TODO:多分いらない？
            //// RazorPagesを使用する設定
            //// ルーティング設定
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: RootsName,    // ルート名
            //        template: RootsTemplate);    // URIパターン(デフォルト値付きで設定、defalts:パラメータは使用しない)
            //                                     // id?は任意に設定できるパラメータとなる
            //});

            // ポリシーで分岐するときに必要？しないなら削除
            //// cookieポリシーを使用する
            //// これをUseMvc()より前に書くと、クライアントに提供するCookieが渡されないのでセッションが維持できない。
            //app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<ChatHub>("/chatHub");
            //});
        }
    }
}
