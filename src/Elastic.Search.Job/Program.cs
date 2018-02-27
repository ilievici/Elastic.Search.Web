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

            var batchRows = configuration.GetSection("batchRows")
                .Get<int>();

            foreach (var clientId in clientIds)
            {
                Console.WriteLine($"CLEINT: {clientId}");

                using (var scope = container.BeginLifetimeScopeForClient(clientId))
                {
                    var indexConfigProvider = scope.Resolve<IIndexConfigProvider>();
                    var paymentService = scope.Resolve<IPaymentService>();

                    if (!await indexConfigProvider.IndexExists())
                    {
                        await indexConfigProvider.CreateIndex<ElasticPaymentModel>();
                    }

                    try
                    {
                        var repository = scope.Resolve<IPaymentsRepository>();

                        
                        int lastPaymentId = 0;
                        var maxPaymentStr = await paymentService.GetMaxPaymentId();
                        if (!string.IsNullOrWhiteSpace(maxPaymentStr))
                        {
                            int.TryParse(maxPaymentStr, out lastPaymentId);
                        }

                        int startRow = 0;
                        int endRow = batchRows;
                        int totalLeft = repository.GetTotalPayments(lastPaymentId);

                        do
                        {
                            var payments = repository.GetPayments(sql, startRow, endRow, lastPaymentId);

                            startRow = endRow;
                            endRow += batchRows;

                            if (endRow > totalLeft)
                            {
                                endRow = totalLeft;
                            }

                            if (payments.Count == 0)
                            {
                                break;
                            }

                            Console.Title = (totalLeft - endRow).ToString();
                            Console.WriteLine($" TOTAL PAYMENTS: {payments.Count}");
                            paymentService.BulkInsert(payments);
                        } while (endRow <= totalLeft);
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
