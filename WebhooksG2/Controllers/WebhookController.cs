using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webhook.Application.Webhooks.Commands;
using Webhook.Application.Webhooks.Queries;

namespace Webhook.API.Controllers
{
    public class WebhookController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] CreateWebhookCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetWebhooksQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerWebhooks([FromBody] TriggerWebhooksCommand request)
        {
            return Ok(await Mediator.Send(request));

        }
    }
}

