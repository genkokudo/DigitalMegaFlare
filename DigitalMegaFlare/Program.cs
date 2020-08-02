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
            Console.WriteLine($"���݂̊���{envName}�ł��B");

            //// �����ł�appsettings.json��ǂ݂����ꍇ
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true)
            //    .AddJsonFile($"appsettings.{envName}.json")
            //    .Build();

            // Web�z�X�g�쐬
            var host = CreateWebHostBuilder(args).Build();

            // DB�ɏ����l��o�^
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // DB���}�C�O���[�V��������
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    // �}�X�^�f�[�^�̏��������K�v�ȏꍇ�A���������N���X���쐬����
                    //DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            // �����f�[�^���Ȃ���΍쐬����
            //CreateRole(host);

            host.Run();
        }

        #region �G�Ȍ����쐬
        ///// <summary>
        ///// �����f�[�^��������΍쐬����
        ///// </summary>
        ///// <param name="host">Web��Host�i�K�v�ȃT�[�r�X�擾�Ɏg�p�j</param>
        //private static void CreateRole(IWebHost host)
        //{
        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var serviceProvider = scope.ServiceProvider;
        //        // �����Ǘ����擾
        //        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //        var role = SystemConstants.Administrator; // �Ǘ��Ҍ���

        //        try
        //        {
        //            Task<IdentityResult> roleResult;
        //            // �Ǘ��Ҍ��������邩�`�F�b�N
        //            Task<bool> hasRole = roleManager.RoleExistsAsync(role);
        //            hasRole.Wait();

        //            if (!hasRole.Result)
        //            {
        //                // ������Βǉ�
        //                roleResult = roleManager.CreateAsync(new IdentityRole(role));
        //                roleResult.Wait();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        //            logger.LogError(ex, "�����쐬�Ɏ��s���܂����B");
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// WebHost���쐬���܂��B
        /// 3.0�ȍ~�̏�����
        /// </summary>
        /// <param name="args"></param>
        /// <param name="port">�g�p����|�[�g</param>
        /// <returns></returns>
        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // NLog �ȊO�Őݒ肳�ꂽ Provider �̖�����.
                    logging.ClearProviders();
                    // �ŏ����O���x���̐ݒ�.
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                // NLog ��L���ɂ���.
                .UseNLog();

        ///// <summary>
        ///// ���̃z�X�e�B���O�̕��@���ƁAEFCore�̃}�C�O���[�V�����ŃG���[���o�邽��
        ///// ���̂悤��Factory�N���X����������
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
