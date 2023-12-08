using Microsoft.EntityFrameworkCore;
namespace Webhook.Application.Common.Interfaces
{
    public interface IWebhookDbContext
    {
        DbSet<Domain.Entities.Webhook> Webhooks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

