using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PosCFG.JPOS;

namespace PosCFG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //jpos j=new jpos();
            //j.setSysconfigValue_CA("Handy Prueba             Montevideo   UY,1234,12345678901,02 215500380016,79050000000000000011");
            //Console.WriteLine(j.genSysconfigValue_CA());
            //Console.WriteLine(new string((new char[]{})).Length);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
