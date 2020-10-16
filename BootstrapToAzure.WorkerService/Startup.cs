using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BootstrapToAzure.Business;
using BootstrapToAzure.Common;
using BootstrapToAzure.Data;
using Microsoft.Extensions.Logging;

namespace BootstrapToAzure.WorkerService
{
    public static class Startup
    {
        public static void ConfigureAppConfiguration(HostBuilderContext hostContext, IConfigurationBuilder configurationBuilder)
        {
            Console.WriteLine("Start loading ConfigureAppConfiguration");

            //configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            //configurationBuilder.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            Console.WriteLine("Finished loading ConfigureAppConfiguration");
        }

        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection servicesCollection)
        {
            Console.WriteLine("Start loading ConfigureServices");

            servicesCollection.AddLogging
                (
                    opt =>
                    {
                        opt.AddConsole
                        (
                            c =>
                            {
                                c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                            }
                        );
                    }
                );

            //Configuration
            servicesCollection.Configure<IConfiguration>(hostContext.Configuration);
            servicesCollection.Configure<GeneralConfiguration>(hostContext.Configuration.GetSection(GeneralConfiguration.SectionName));
            servicesCollection.Configure<AzureConfiguration>(hostContext.Configuration.GetSection(AzureConfiguration.SectionName));
            servicesCollection.Configure<VericoinConfiguration>(hostContext.Configuration.GetSection(VericoinConfiguration.SectionName));
            servicesCollection.Configure<VeriumConfiguration>(hostContext.Configuration.GetSection(VeriumConfiguration.SectionName));

            //UI
            servicesCollection.AddHostedService<Worker>();

            //Buiness
            servicesCollection.AddTransient<IMainHandler, MainHandler>();
            servicesCollection.AddTransient<ICryptoHandler, CryptoHandler>();

            //Data
            servicesCollection.AddTransient<IBlobUploadHandler, BlobUploadHandler>();
            servicesCollection.AddTransient<IDockerHandler, DockerHandler>();
            servicesCollection.AddTransient<IFileHandler, FileHandler>();

            Console.WriteLine("Finished loading ConfigureServices");
        }
    }
}
