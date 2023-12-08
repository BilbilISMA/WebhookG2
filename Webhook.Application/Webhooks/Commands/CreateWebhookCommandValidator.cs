using FluentValidation;
using Webhook.Domain.Enums;

namespace Webhook.Application.Webhooks.Commands
{
    public class CreateWebhookCommandValidator : AbstractValidator<CreateWebhookCommand>
    {
        public CreateWebhookCommandValidator()
        {
            RuleFor(command => command.HTTPMethod)
                .IsInEnum().WithMessage("Method not valid");          

            RuleFor(command => command.Url)
            .NotEmpty().WithMessage("URL is required.")
            .Must(BeAValidUrl).WithMessage("Invalid URL format.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
