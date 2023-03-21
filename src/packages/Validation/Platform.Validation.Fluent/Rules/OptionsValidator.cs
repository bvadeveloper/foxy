using FluentValidation;
using Platform.Validation.Fluent.Messages;

namespace Platform.Validation.Fluent.Rules
{
    public class OptionsValidator : AbstractValidator<OptionValidationMessage>
    {
        public OptionsValidator()
        {
            RuleFor(model => model.Value)
                .NotNull()
                .NotEmpty()
                .Custom((value, context) =>
                {
                    if (string.IsNullOrWhiteSpace(value)
                        || value.Length > 144
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
                        context.AddFailure($"I see invalid characters in options '{value}'");
                    }
                });
        }
    }
}