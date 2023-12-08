using System.ComponentModel;
using System.Runtime.Serialization;

namespace Webhook.Domain.Enums
{
    public enum HTTPMethod
	{
		GET = 1,
        POST,
        PUT,
        DELETE,
        PATCH
	}
}

