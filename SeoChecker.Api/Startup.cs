using Microsoft.OpenApi.Models;
using SeoChecker.Api;
using SeoChecker.Api.ActionFilters;
using SeoChecker.Api.Interfaces;
using SeoChecker.Api.Middlewares;
using SeoChecker.Api.Services;
using SeoChecker.Shared.Models;

namespace SeoChecker
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new ValidateModelAttribute());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SeoChecker API", Version = "v1" });
            });
            services.AddHttpClient<GoogleSearchService>();
            services.AddHttpClient<BingSearchService>();
            services.AddMemoryCache();
            services.AddHostedService<CacheBackgroundService>();
            services.Configure<DefaultSearchRequest>(Configuration.GetSection("DefaultSearchRequest"));
            services.AddSingleton<ISearchServiceFactory, SearchServiceFactory>();
            services.AddScoped<IHandler, SearchHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SeoChecker API");
                    c.RoutePrefix = string.Empty; // Put Swagger UI at root, ex: http://localhost:5000/
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}

