using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using Webhook.Application.Webhooks.Commands;
using Webhook.Application.Webhooks.Queries;
using Webhook.Application.Common.Behaviours;
using FluentValidation;

namespace Webhook.Application
{
    public static class DependencyInjection
	{
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            // Register your command handlers
            services.AddScoped<IRequestHandler<CreateWebhookCommand, Unit>, CreateWebhookCommandHandler>();
            services.AddScoped<IRequestHandler<TriggerWebhooksCommand, Unit>, TriggerWebhooksCommandHandler>();

            // Register your query handlers
            services.AddScoped<IRequestHandler<GetWebhooksQuery, List<Domain.Entities.Webhook>>, GetWebhooksQueryHandler>();


            return services;
        }
    }
}

