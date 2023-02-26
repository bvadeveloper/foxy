using FluentValidation.Results;
using Platform.Contract.Abstractions;

namespace Platform.Validation.Fluent.Abstractions;

public interface IValidationFactory
{
    ValidationResult Validate(ITarget target);
}