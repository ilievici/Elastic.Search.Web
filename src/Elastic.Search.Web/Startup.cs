using System;
using System.IO;
using System.Reflection;
using Elastic.Search.Core.Filter;
using Elastic.Search.Core.Infrastructure;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service;
using Elastic.Search.Core.Service.Abstract;
using Elastic.Search.Web.Infrastructure;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Nest;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Elastic.Search.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                    options.Filters.Add(typeof(ModelValidationFilter));
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Info
                {
                    Title = "Web HTTP API",
                    Version = "v1.0",
                    Description = "Paymnet Search Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });


                // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                // 
                options.IgnoreObsoleteActions();

                // Set this flag to omit schema property descriptions for any type properties decorated with the
                // Obsolete attribute 
                //
                options.IgnoreObsoleteProperties();

                // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                // 
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();

                // Integrate XML comments
                if (File.Exists(XmlCommentsFilePath))
                    options.IncludeXmlComments(XmlCommentsFilePath);
            });

            services.AddOptions();
            services.Configure<ElasticConnectionOption>(Configuration);

            services.AddSingleton<IElasticClient, ElasticClient>(s =>
            {
                var connectionString = Configuration.GetSection("elasticConnectionOption")
                    .Get<ElasticConnectionOption>();

                if (connectionString == null)
                {
                    //TODO: this is only for test in case of missing configurations in appsettings.json
                    connectionString = new ElasticConnectionOption("http", "localhost", 9200);
                }

                var node = new UriBuilder(connectionString.Scheme, connectionString.Host, connectionString.Port);

                var connectionPool = new SingleNodeConnectionPool(node.Uri);
                var settings = new ConnectionSettings(connectionPool);

                //Ensures a full log from ES engine
                settings.DisableDirectStreaming();

                return new ElasticClient(settings);
            });

            //WebAPI
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();

            //Core 
            services.AddScoped<IIndexConfigProvider, IndexConfigProvider>();

            //Services
            services.AddScoped<IPaymentSearchService, PaymentSearchService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<ISecuritySettingsService, SecuritySettingsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Configure logs
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ElasticPaymentModel Search API v1.0");

                    c.BooleanValues(new object[] { 0, 1 });
                    c.DocExpansion("none");
                    c.SupportedSubmitMethods(new[] { "get", "post", "put", "patch", "delete" });
                });
        }

        /// <summary>
        /// XML Comments File path
        /// </summary>
        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
