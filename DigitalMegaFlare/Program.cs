using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace DigitalMegaFlare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            // �����ł�appsettings.json��ǂ݂���
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // �g�p����|�[�g
            var port = "http://0.0.0.0:" + config.GetValue<string>(SystemConstants.Port);

            // Web�z�X�g�쐬
            var host = CreateWebHostBuilder(args).UseUrls(port).Build();

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

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        // Core1�n�Ŗ��񏑂��Ă����R�[�h�����b�v���Ă���
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // NLog �ȊO�Őݒ肳�ꂽ Provider �̖�����.
                    logging.ClearProviders();
                    // �ŏ����O���x���̐ݒ�.
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                // NLog ��L���ɂ���.
                .UseNLog();
    }
}