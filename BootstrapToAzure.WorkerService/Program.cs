using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BootstrapToAzure.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting up");

                IHostBuilder hostBuilder = CreateHostBuilder(args);
                IHost host = hostBuilder.Build();
                host.Run();

                Console.WriteLine($"Shutting down");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during boot: {ex.Message}");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(Startup.ConfigureAppConfiguration)
                .ConfigureServices(Startup.ConfigureServices);
    }
}



//docker run -it --name test -v /var/run/docker.sock:/var/run/docker.sock -v /var/lib/docker/volumes/veriumminer_root_home/_data/:/root/veriumminer/ -v /var/lib/docker/volumes/verium_root_home/_data/:/root/verium/ -v /var/lib/docker/volumes/vericoin_root_home/_data/:/root/vericoin/ bootstraptoazureworkerservice bash

//docker run -d --name test -v /var/run/docker.sock:/var/run/docker.sock -v /var/lib/docker/volumes/veriumminer_root_home/_data/:/root/veriumminer/ -v /var/lib/docker/volumes/verium_root_home/_data/:/root/verium/ -v /var/lib/docker/volumes/vericoin_root_home/_data/:/root/vericoin/ bootstraptoazureworkerservice
