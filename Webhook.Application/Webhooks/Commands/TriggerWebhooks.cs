using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Webhook.Application.Common.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Webhook.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Webhook.Application.Webhooks.Commands
{
    // Command
    public class TriggerWebhooksCommand : IRequest<Unit>
    {
	}

    //Command Handler
    public class TriggerWebhooksCommandHandler : IRequestHandler<TriggerWebhooksCommand, Unit>
    {
        private readonly IWebhookDbContext _dbContext;
        private readonly ILogger<TriggerWebhooksCommand> _logger;

        public TriggerWebhooksCommandHandler(IWebhookDbContext dbContext, ILogger<TriggerWebhooksCommand> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(TriggerWebhooksCommand request, CancellationToken cancellationToken)
        {
            // Get data to send
            //This is hardcoded for testing purposes but it should be the updated data according to the webhook data schema
            var dataToSend = new UserDetails();

            // Get list of registered webhooks
            var webhooks = await _dbContext.Webhooks.ToListAsync(cancellationToken);

            // Create a list of tasks for sending webhook requests
            var tasks = webhooks.Select(webhook => CreateWebhookRequestAsync(webhook.Url, webhook.Method, webhook.JsonSchema, dataToSend, cancellationToken)).ToList();

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        #region Private methods


        private async Task CreateWebhookRequestAsync(string url, HTTPMethod method, string jsonSchema, UserDetails userDetails,  CancellationToken cancellationToken)
        {
            try
            {
                //Generate payload based on the configured Json Schema of the webhook            
                var payload = GeneratePayload(jsonSchema, userDetails);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                //await client.PostAsync("https://webhook.site/41f849ba-eca5-45bd-94b6-321db96fb6b4", content); //Using hardcoded url for testing purposes 
                await SendRequestAsync(url, method, content, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Creating webhook request failed", ex);
                throw;
            }            
        }

        public class UserDetails
        {
            public string FirstName { get; set; } = "John";
            public string LastName { get; set; } = "Doe";
            public string Email { get; set; } = "john.doe@email.com";
            public string Username { get; set; } = "john.doe";
            public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-22);
            public string Country { get; set; } = "United Kingdom";
        }

        private string GeneratePayload(string webhookSchema, UserDetails userDetails)
        {
            try
            {
                // Deserialize JSON schema
                JSchema schema = JSchema.Parse(webhookSchema);

                // Create a JObject for the new JSON payload
                JObject jsonPayload = new JObject();

                // Iterate through the properties of UserDetails
                var test = typeof(UserDetails).GetProperties();
                foreach (PropertyInfo propertyInfo in typeof(UserDetails).GetProperties())
                {
                    // Check if the property is defined in the schema
                    if (schema.Properties.TryGetValue(propertyInfo.Name, out var schemaProperty))
                    {
                        // Map the property value to the corresponding schema attribute
                        jsonPayload[propertyInfo.Name] = JToken.FromObject(propertyInfo.GetValue(userDetails));
                    }
                }

                // Convert the JObject to a JSON string
                return jsonPayload.ToString();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Generating payload failed", ex);
                throw;
            }
            
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string url, HTTPMethod method, HttpContent content, CancellationToken cancellationToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;

                    switch (method)
                    {
                        case HTTPMethod.GET:
                            response = await client.GetAsync(url, cancellationToken);
                            break;
                        case HTTPMethod.POST:
                            response = await client.PostAsync(url, content, cancellationToken);
                            break;
                        case HTTPMethod.PUT:
                            response = await client.PutAsync(url, content, cancellationToken);
                            break;
                        case HTTPMethod.DELETE:
                            response = await client.DeleteAsync(url, cancellationToken);
                            break;
                        case HTTPMethod.PATCH:
                            response = await client.PatchAsync(url, content, cancellationToken);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(method), method, null);
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sending request failed", ex);
                throw;
            } 
        }

        #endregion
    }
}

