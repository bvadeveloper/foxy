using FluentValidation.Results;
using Platform.Validation.Fluent.Messages;

namespace Platform.Validation.Fluent;

public interface IValidationFactory
{
    ValidationResult Validate(ITelegramValidationMessage message);
}