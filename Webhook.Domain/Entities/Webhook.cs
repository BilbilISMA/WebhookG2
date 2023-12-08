using Webhook.Domain.Common;
using Webhook.Domain.Enums;

namespace Webhook.Domain.Entities
{
    public class Webhook : BaseAuditableEntity
	{
		public required string Url { get; set; }
		public HTTPMethod Method { get; set; }
        public string? Description { get; set; }
		public string? JsonSchema { get; set; }
	}
}

