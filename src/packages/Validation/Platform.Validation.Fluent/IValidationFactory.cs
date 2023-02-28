using FluentValidation.Results;

namespace Platform.Validation.Fluent;

public interface IValidationFactory
{
    ValidationResult Validate(string message);
}