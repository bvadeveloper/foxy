using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Platform.Contract.Abstractions;
using Platform.Logging.Extensions;
using Platform.Validation.Fluent.Abstractions;

namespace Platform.Validation.Fluent;

public class ValidationFactory : IValidationFactory
{
    private readonly Type[] _types;
    private readonly ILogger _logger;

    public ValidationFactory(ILogger<ValidationFactory> logger)
    {
        _logger = logger;
        _types = Assembly.GetExecutingAssembly().GetTypes();
    }

    public IImmutableList<ITarget> Validate(IImmutableList<ITarget> targets) =>
        targets
            .Where(target =>
            {
                var genericValidationType = typeof(IValidator<>).MakeGenericType(target.GetType());
                var methodInfo = genericValidationType.GetMethod("Validate");
                var validatorType = _types.FirstOrDefault(x =>
                    genericValidationType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

                var instance = Activator.CreateInstance(validatorType);
                var validationResult = (ValidationResult)methodInfo.Invoke(instance, BindingFlags.Public, null,
                    new[] { target }, CultureInfo.InvariantCulture);

                if (!validationResult.IsValid)
                {
                    _logger.Warn($"Validation failed for '{target.Target}', '{validationResult.Errors.ToString()}'");
                }

                return validationResult.IsValid;
                
            }).ToImmutableList();
}