﻿using FluentValidation;
using Platform.Contract;

namespace Platform.Validation.Fluent.Rules
{
    public class DomainTargetValidator : AbstractValidator<DomainTarget>
    {
        public DomainTargetValidator()
        {
            RuleFor(model => model.Value)
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