using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Money.Api.Extensions;
using Money.Api.Interfaces;
using Money.Api.Interfaces.Api;
using Money.Api.Interfaces.Providers;
using Money.Api.Middleware;
using Money.Api.Models.Settings;
using Money.Api.Services;
using Refit;

namespace Money.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dollarSettings = Configuration.GetSection(nameof(DollarSettings)).Get<DollarSettings>();
            services.AddRefitClient<IBancoProvinciaApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(dollarSettings.BaseUrl));
            services.Configure<TransactionSettings>(Configuration.GetSection(nameof(TransactionSettings)));
            services.AddScoped<ITodayDollarPriceProvider, DollarService>();
            services.AddScoped<ITodayRealPriceProvider, RealService>();
            services.AddScoped<IValidatorService, ValidatorService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPriceProvider>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<TodayPriceProvider>>();
                var validator = provider.GetRequiredService<IValidatorService>();
                var service = new TodayPriceProvider(logger, validator);
                service.AddPriceProvider(provider.GetRequiredService<ITodayDollarPriceProvider>());
                service.AddPriceProvider(provider.GetRequiredService<ITodayRealPriceProvider>());
                return service;
            });
            services.AddApp(Configuration.GetConnectionString("DefaultConnection"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplyMigrations();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(options =>
                options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}
