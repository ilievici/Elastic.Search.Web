using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Microsoft.Extensions.Configuration;

namespace Elastic.Search.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            Dojob().Wait();

            Console.WriteLine("Done!");
            Thread.Sleep(5000);
        }

        static async Task Dojob()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("job-settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var clientIds = configuration.GetSection("clientIds")
                .Get<List<string>>();

            if (!clientIds.Any())
                return;

            var container = new Startup().BuildContainer(configuration);
            
            string sql;
            try
            {
                var scriptPath = configuration.GetSection("scriptPath").Get<string>();
                if (string.IsNullOrWhiteSpace(scriptPath))
                    return;

                sql = File.ReadAllText(scriptPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (string.IsNullOrWhiteSpace(sql))
                return;

            foreach (var clientId in clientIds)
            {
                Console.WriteLine($"CLEINT: {clientId}");

                using (var scope = container.BeginLifetimeScopeForClient(clientId))
                {
                    try
                    {
                        var payments = scope.Resolve<IPaymentsRepository>().GetPayments(sql);
                        if (!payments.Any())
                        {
                            continue;
                        }
                        Console.WriteLine($" TOTAL PAYMENTS: {payments.Count}");

                        var indexConfigProvider = scope.Resolve<IIndexConfigProvider>();
                        var paymentService = scope.Resolve<IPaymentService>();

                        if (await indexConfigProvider.IndexExists())
                        {
                            await indexConfigProvider.DeleteIndex();
                        }

                        await indexConfigProvider.CreateIndex<ElasticPaymentModel>();
                        await paymentService.BulkInsert(payments);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}
