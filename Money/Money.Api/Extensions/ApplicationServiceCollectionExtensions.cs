using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Money.Api.Interfaces;
using Money.Api.Models;
using Newtonsoft.Json;
using Serilog;

namespace Money.Api.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApp(this IServiceCollection services, string connectionString)
        {
            services.AddCors()
                .AddMvcCore()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                })
                .AddApiExplorer()
                .AddAuthorization();

            services.AddHttpContextAccessor();
            services.AddControllers();

            return services.AddFluentValidation().AddAppDbContext(connectionString);
        }

        private static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<IValidatorService>();
            services.AddScoped<IValidatorFactory, ServiceProviderValidatorFactory>();

            return services;
        }

        private static IServiceCollection AddAppDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
#if DEBUG
                options.UseLoggerFactory(LoggerFactory.Create(c => c.AddSerilog()));
                options.EnableSensitiveDataLogging();
#endif
                options.UseSqlServer(connectionString, o => o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
            return services;
        }

    }
}
