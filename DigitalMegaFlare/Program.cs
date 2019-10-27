using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DigitalMegaFlare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();


            //// ここでもappsettings.jsonを読みたい
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true)
            //    .Build();

            //// 使用するポート
            //var port = "http://0.0.0.0:" + config.GetValue<string>(SystemConstants.Port);

            //// Webホスト作成
            //var host = CreateWebHostBuilder(args).UseUrls(port).Build();

            //// 権限データがなければ作成する
            //CreateRole(host);

            //host.Run();
        }

        #region 雑な権限作成
        ///// <summary>
        ///// 権限データが無ければ作成する
        ///// </summary>
        ///// <param name="host">WebのHost（必要なサービス取得に使用）</param>
        //private static void CreateRole(IWebHost host)
        //{
        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var serviceProvider = scope.ServiceProvider;
        //        // 権限管理を取得
        //        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //        var role = SystemConstants.Administrator; // 管理者権限

        //        try
        //        {
        //            Task<IdentityResult> roleResult;
        //            // 管理者権限があるかチェック
        //            Task<bool> hasRole = roleManager.RoleExistsAsync(role);
        //            hasRole.Wait();

        //            if (!hasRole.Result)
        //            {
        //                // 無ければ追加
        //                roleResult = roleManager.CreateAsync(new IdentityRole(role));
        //                roleResult.Wait();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        //            logger.LogError(ex, "権限作成に失敗しました。");
        //        }
        //    }
        //}
        #endregion

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //// Core1系で毎回書いていたコードをラップしている
        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .ConfigureLogging((hostingContext, logging) => {
        //            // NLog 以外で設定された Provider の無効化.
        //            logging.ClearProviders();
        //            // 最小ログレベルの設定.
        //            logging.SetMinimumLevel(LogLevel.Trace);
        //        })
        //        // NLog を有効にする.
        //        .UseNLog();
    }
}
