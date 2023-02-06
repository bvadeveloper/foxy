using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Platform.Contract.Abstractions;
using Platform.Validation.Fluent.Abstractions;

namespace Platform.Validation.Fluent;

public class ValidationFactory : IValidationFactory
{
    private readonly Type[] _types;

    public ValidationFactory() => _types = Assembly.GetExecutingAssembly().GetTypes();

    public IImmutableList<(ITarget, bool)> Validate(IImmutableList<ITarget> targets) =>
        targets
            .Select(target =>
            {
                var genericValidationType = typeof(IValidator<>).MakeGenericType(target.GetType());
                var methodInfo = genericValidationType.GetMethod("Validate");
                var validatorType = _types.FirstOrDefault(x =>
                    genericValidationType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
                var instance = Activator.CreateInstance(validatorType);
                var result = (ValidationResult)methodInfo.Invoke(instance, BindingFlags.Public, null,
                    new[] { target }, CultureInfo.InvariantCulture);

                return (target, result.IsValid);
                
            }).ToImmutableList();
}