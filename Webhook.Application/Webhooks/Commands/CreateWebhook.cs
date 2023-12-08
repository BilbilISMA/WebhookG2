using System.ComponentModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webhook.Application.Common.Interfaces;
using Webhook.Domain.Enums;

namespace Webhook.Application.Webhooks.Commands
{
    //Command
    public class CreateWebhookCommand : IRequest<Unit>
    {
        [DefaultValue("https://webhook.site/41f849ba-eca5-45bd-94b6-321db96fb6b4")]
        public required string Url { get; set; }
        public required HTTPMethod HTTPMethod { get; set; }
        public string? JsonSchema { get; set; } = @"
        {
          ""type"": ""object"",
          ""properties"": {
            ""FirstName"": { ""type"": ""string"" },
            ""LastName"": { ""type"": ""string"" },
            ""Email"": { ""type"": ""string"" },
            ""DateOfBirth"": { ""type"": ""string"", ""format"": ""date-time"" }
          }
        }";

        public string? Description { get; set; }
    }

    // Command Handler
    public class CreateWebhookCommandHandler : IRequestHandler<CreateWebhookCommand, Unit>
    {
        private readonly IWebhookDbContext _dbContext;
        private readonly ILogger<CreateWebhookCommand> _logger;

        public CreateWebhookCommandHandler(IWebhookDbContext dbContext, ILogger<CreateWebhookCommand> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateWebhookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var webhookEndpoint = new Domain.Entities.Webhook
                {
                    Url = request.Url,
                    Method = request.HTTPMethod,
                    JsonSchema = request.JsonSchema,
                    Description = request.Description
                };

                _dbContext.Webhooks.Add(webhookEndpoint);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Error saving webhook to the database", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred", ex);
                throw;
            }
        }
    }
}

