using FluentValidation;
using Platform.Contract.Telegram;

namespace Platform.Validation.Fluent.Rules
{
    public class DomainValidator : AbstractValidator<DomainMessage>
    {
        public DomainValidator()
        {
            RuleFor(model => model.Name)
                .NotNull()
                .NotEmpty()
                .Custom(DomainNameStaticValidator.Validate)
                .Custom((text, context) =>
                {
                    if (string.IsNullOrWhiteSpace(text)
                        || text.Length > 1024
                        || text.Contains('%')
                        || text.Contains('*')
                        || text.Contains('|')
                        || text.Contains('\\')
                        || text.Contains('#')
                        || text.Contains('&')
                        || text.Contains('^')
                        || text.Contains(')')
                        || text.Contains('(')
                        || text.Contains('!')
                        || text.Contains('+')
                        || text.Contains('~')
                        || text.Contains('"')
                        || text.Contains('`')
                        || text.Contains('$'))
                    {
                        context.AddFailure("not valid symbols caught");
                    }
                });
        }
    }
}