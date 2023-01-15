using System.Collections.Immutable;
using Platform.Contract.Abstractions;

namespace Platform.Validation.Fluent.Abstractions;

public interface IValidationFactory
{
    IImmutableList<(ITarget, bool)> Validate(IImmutableList<ITarget> targets);
}