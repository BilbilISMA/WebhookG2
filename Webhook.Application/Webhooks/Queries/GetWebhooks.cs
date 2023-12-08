using MediatR;
using Microsoft.EntityFrameworkCore;
using Webhook.Application.Common.Interfaces;

namespace Webhook.Application.Webhooks.Queries
{
    // Query
    public class GetWebhooksQuery : IRequest<List<Domain.Entities.Webhook>>
    {
	}

    // Query Handler
    public class GetWebhooksQueryHandler : IRequestHandler<GetWebhooksQuery, List<Domain.Entities.Webhook>>
    {
        private readonly IWebhookDbContext _dbContext;

        public GetWebhooksQueryHandler(IWebhookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Domain.Entities.Webhook>> Handle(GetWebhooksQuery request, CancellationToken cancellationToken)
        {
            var webhooks = await _dbContext.Webhooks.ToListAsync(cancellationToken);
            return webhooks;
        }
    }
}

