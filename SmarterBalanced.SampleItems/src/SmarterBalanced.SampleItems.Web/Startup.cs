using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SmarterBalanced.SampleItems.Core.Diagnostics;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Core.ScoreGuide;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.PlatformAbstractions;
using SmarterBalanced.SampleItems.Core.Reporting;

namespace SmarterBalanced.SampleItems.Web
{
    public class Startup
    {
        private readonly ILogger logger;
        public Startup(IHostingEnvironment env, ILoggerFactory factory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
            ConfigureLogging(env, factory);
            logger = factory.CreateLogger<Startup>();

        }

        private void ConfigureLogging(IHostingEnvironment env, ILoggerFactory factory)
        {
            factory.AddConsole(Configuration.GetSection("Logging"));
            factory.AddDebug();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            SampleItemsContext context;
            AppSettings appSettings = new AppSettings();

            Configuration.Bind(appSettings);
            try
            {
                context = SampleItemsProvider.LoadContext(appSettings, logger);
            }
            catch (Exception e)
            {
                logger.LogCritical($"{e.Message} occurred when loading the context");
                throw e;
            }

            services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddMvc();
            services.AddRouting();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Item Sampler API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "SmarterBalanced.SampleItems.Web.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => 
                    builder.AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod());
            });


            services.AddSingleton(context);
            services.AddSingleton(appSettings);
            services.AddScoped<IItemViewRepo, ItemViewRepo>();
            services.AddScoped<ISampleItemsSearchRepo, SampleItemsSearchRepo>();
            services.AddScoped<IAboutItemsRepo, AboutItemsRepo>();
            services.AddScoped<IDiagnosticManager, DiagnosticManager>();
            services.AddScoped<IScoringRepo, ScoringRepo>();
            services.AddScoped<IReportingRepo, ReportingRepo>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
           
                app.UseStaticFiles();

                //app.UseDeveloperExceptionPage();
                //app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithRedirects("/Home/StatusCodeError?code={0}");
            }


            app.UseStaticFiles();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "diagnostic",
                    template: "status/{level?}",
                    defaults: new { controller = "Diagnostic", action = "Index" });
            });

            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/swagger"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Home", action = "Index" });
                });
            });


            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ItemSampler");
            });

        }

    }
}
