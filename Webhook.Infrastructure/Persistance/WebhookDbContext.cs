using System;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Webhook.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Webhook.Infrastructure.Persistance
{
	public class WebhookDbContext : DbContext, IWebhookDbContext
    {
        private readonly IConfigurationRoot _configuration;

        public WebhookDbContext()
        {

        }
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options, IConfigurationRoot configuration) : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<Domain.Entities.Webhook> Webhooks => Set<Domain.Entities.Webhook>();

        public DbSet<T> EntityAsDbSet<T>() where T : class => Set<T>();
        public IModel ModelMetadata() => Model;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                connectionString = new SqliteConnectionStringBuilder(connectionString)
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ToString();
                optionsBuilder.UseSqlite(connectionString);
            }
#if (DEBUG)
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.EnableDetailedErrors(true);
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
#endif
        }
    }
}

