using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WeatherMicroservice.Services;
using Microsoft.FeatureManagement;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using App.Metrics;
using Prometheus;
using Sentry;
using Serilog;
using Serilog.Sinks.Elasticsearch;


namespace WeatherMicroservice
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
            services.AddControllers();
            services.AddScoped<IWeatherService, WeatherService>();

            // Register the Feature Management services
            services.AddFeatureManagement(Configuration.GetSection("FeatureManagement"));

            services.AddHealthChecks();
            services.AddEndpointsApiExplorer();
            services.AddApplicationInsightsTelemetry();
            services.AddMetrics();

            // Register the HttpClientFactory
            services.AddHttpClient();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherMicroservice", Version = "v1" });
            });

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                SentrySdk.CaptureMessage("Hello Sentry");
                app.UseDeveloperExceptionPage();
            }

            app.UseSentryTracing();

            app.UseHttpMetrics();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapMetrics();
            });

            // Add Prometheus metrics endpoint
            app.UseMetricServer();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherMicroservice V1");
            });
        }
    }
}
