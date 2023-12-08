using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webhook.Infrastructure.Persistance.Configurations
{
    public class WebhookConfiguration : IEntityTypeConfiguration<Domain.Entities.Webhook>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Webhook> builder)
        {
            builder.Property(t => t.Url)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}

