using DigitalMegaFlare.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace DigitalMegaFlare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Console.WriteLine($"現在の環境は{envName}です。");

            //// ここでもappsettings.jsonを読みたい場合
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true)
            //    .AddJsonFile($"appsettings.{envName}.json")
            //    .Build();

            // Webホスト作成
            var host = CreateWebHostBuilder(args).Build();

            // DBに初期値を登録
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // DBをマイグレーションする
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    // マスタデータの初期化が必要な場合、こういうクラスを作成する
                    //DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            // 権限データがなければ作成する
            //CreateRole(host);

            host.Run();
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

        /// <summary>
        /// WebHostを作成します。
        /// 3.0以降の書き方
        /// </summary>
        /// <param name="args"></param>
        /// <param name="port">使用するポート</param>
        /// <returns></returns>
        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // NLog 以外で設定された Provider の無効化.
                    logging.ClearProviders();
                    // 最小ログレベルの設定.
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                // NLog を有効にする.
                .UseNLog();

        ///// <summary>
        ///// このホスティングの方法だと、EFCoreのマイグレーションでエラーが出るため
        ///// このようなFactoryクラスを準備する
        ///// </summary>
        //public class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        //{
        //    public ApplicationDbContext CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //        optionsBuilder.UseSqlServer("Server=localhost;Database=digitalmegaflare;User Id=ginpay;Password=password;"
        //            );

        //        //optionsBuilder.UseMySql("Server=localhost;Database=digitalmegaflare;User Id=ginpay;Password=password;",
        //        //    mySqlOptions =>
        //        //    {
        //        //        mySqlOptions.ServerVersion(new Version(10, 3, 13), ServerType.MariaDb);
        //        //    }
        //        //);
        //        return new ApplicationDbContext(optionsBuilder.Options);
        //    }
        //}
    }
}
