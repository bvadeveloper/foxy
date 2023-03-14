using FluentValidation;
using Platform.Validation.Fluent.Messages;

namespace Platform.Validation.Fluent.Rules
{
    public class DomainValidator : AbstractValidator<DomainValidationMessage>
    {
        public DomainValidator()
        {
            RuleForEach(model => model.Value)
                .NotNull()
                .NotEmpty()
                .Custom(DomainNameStaticValidator.Validate)
                .Custom((uri, context) =>
                {
                    if (string.IsNullOrWhiteSpace(uri)
                        || uri.Length > 1024
                        || uri.Contains('%')
                        || uri.Contains('*')
                        || uri.Contains('|')
                        || uri.Contains('\\')
                        || uri.Contains('#')
                        || uri.Contains('&')
                        || uri.Contains('^')
                        || uri.Contains(')')
                        || uri.Contains('(')
                        || uri.Contains('!')
                        || uri.Contains('+')
                        || uri.Contains('~')
                        || uri.Contains('"')
                        || uri.Contains('`')
                        || uri.Contains('$'))
                    {
                        context.AddFailure($"Hmm... I see invalid characters in the domain name: {uri}");
                    }
                });
        }
    }
}