using FluentValidation;
using Platform.Contract.Telegram;

namespace Platform.Validation.Fluent.Rules
{
    public class DomainValidator : AbstractValidator<DomainMessage>
    {
        public DomainValidator()
        {
            RuleForEach(model => model.Value)
                .NotNull()
                .NotEmpty()
                .Custom(DomainNameStaticValidator.Validate)
                .Custom((uri, context) =>
                {
                    var value = uri.ToString();
                    if (string.IsNullOrWhiteSpace(value)
                        || value.Length > 1024
                        || value.Contains('%')
                        || value.Contains('*')
                        || value.Contains('|')
                        || value.Contains('\\')
                        || value.Contains('#')
                        || value.Contains('&')
                        || value.Contains('^')
                        || value.Contains(')')
                        || value.Contains('(')
                        || value.Contains('!')
                        || value.Contains('+')
                        || value.Contains('~')
                        || value.Contains('"')
                        || value.Contains('`')
                        || value.Contains('$'))
                    {
                        context.AddFailure("not valid symbols caught");
                    }
                });
        }
    }
}