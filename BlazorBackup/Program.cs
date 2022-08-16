using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBackup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //ƒобавить чтобы возможность была сапускать приложение как сервис Windows // nuget скачать Microsoft.Extensions.Hosting.WindowsService
            .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //—лужба Windows -получить текущий каталог
                    System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
