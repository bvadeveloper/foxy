using FluentValidation.Results;
using Platform.Contract.Telegram;

namespace Platform.Validation.Fluent;

public interface IValidationFactory
{
    ValidationResult Validate(ITelegramMessage message);
}