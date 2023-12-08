using Ardalis.GuardClauses;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhook.Application.Common.Interfaces;
using Webhook.Infrastructure.Persistance;
using Webhook.Infrastructure.Persistance.Interceptors;

namespace Webhook.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionString = new SqliteConnectionStringBuilder(connectionString)
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();

            Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<WebhookDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IWebhookDbContext>(provider => provider.GetRequiredService<WebhookDbContext>());
            services.AddScoped<WebhookDbContextInitializer>();

            return services;
        }
    }
}
