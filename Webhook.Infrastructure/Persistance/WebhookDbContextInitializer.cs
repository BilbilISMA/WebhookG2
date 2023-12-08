using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace Webhook.Infrastructure.Persistance
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<WebhookDbContextInitializer>();

            await initialiser.InitialiseAsync();

        }
    }

    public class WebhookDbContextInitializer
	{
        private readonly ILogger<WebhookDbContextInitializer> _logger;
        private readonly WebhookDbContext _context;

        public WebhookDbContextInitializer(ILogger<WebhookDbContextInitializer> logger, WebhookDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        // May be also some seeding here
    }
}

