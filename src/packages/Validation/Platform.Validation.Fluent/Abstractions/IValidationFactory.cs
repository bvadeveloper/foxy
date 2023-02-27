using FluentValidation.Results;
using Platform.Contract;
using Platform.Contract.Messages;

namespace Platform.Validation.Fluent.Abstractions;

public interface IValidationFactory
{
    ValidationResult Validate(ITarget target);
}