using System.Collections.Immutable;
using FluentValidation.Results;
using Platform.Contract.Abstractions;

namespace Platform.Validation.Fluent.Abstractions;

public interface IValidationFactory
{
    IImmutableList<ITarget> Validate(IImmutableList<ITarget> targets);
}