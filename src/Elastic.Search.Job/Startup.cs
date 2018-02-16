using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Elastic.Search.Core.Infrastructure;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Service;
using Elastic.Search.Core.Service.Abstract;
using Elastic.Search.Job.Infrastructure;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Elastic.Search.Job
{
    public class Startup
    {
        public IContainer BuildContainer(IConfigurationRoot configuration)
        {
            var services = Services(configuration);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            containerBuilder.RegisterType<ConnectionProvider>()
                .As<IConnectionProvider>()
                .As<ConnectionProvider>()
                .InstancePerLifetimeScope();

            return containerBuilder.Build();
        }

        private IServiceCollection Services(IConfigurationRoot configuration)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(s => configuration);

            services.AddOptions();
            services.Configure<ElasticConnectionOption>(configuration);

            services.AddSingleton<IElasticClient, ElasticClient>(s =>
            {
                var connectionString = configuration.GetSection("elasticConnectionOption")
                    .Get<ElasticConnectionOption>();

                var node = new UriBuilder(connectionString.Scheme, connectionString.Host, connectionString.Port);

                var connectionPool = new SingleNodeConnectionPool(node.Uri);
                var settings = new ConnectionSettings(connectionPool);

                //Ensures a full log from ES engine
                settings.DisableDirectStreaming();

                return new ElasticClient(settings);
            });

            //Core 
            services.AddScoped<IIndexConfigProvider, IndexConfigProvider>();

            //Services
            services.AddScoped<IPaymentSearchService, PaymentSearchService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IPaymentsRepository, PaymentsRepository>();

            return services;
        }
    }
}
