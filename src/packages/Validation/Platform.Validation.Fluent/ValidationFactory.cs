using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Platform.Contract;
using Platform.Contract.Messages;
using Platform.Logging.Extensions;
using Platform.Validation.Fluent.Abstractions;

namespace Platform.Validation.Fluent;

public class ValidationFactory : IValidationFactory
{
    private readonly ILogger<ValidationFactory> _logger;
    private readonly Type[] _types;

    public ValidationFactory(ILogger<ValidationFactory> logger)
    {
        _logger = logger;
        _types = Assembly.GetExecutingAssembly().GetTypes();
    }

    public ValidationResult Validate(ITarget target)
    {
        var genericValidationType = typeof(IValidator<>).MakeGenericType(target.GetType());
        var methodInfo = genericValidationType.GetMethod("Validate");
        var validatorType = _types.FirstOrDefault(x =>
            genericValidationType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
        var instance = Activator.CreateInstance(validatorType);

        var validationResult = (ValidationResult)methodInfo.Invoke(instance, BindingFlags.Public, null,
            new[] { target }, CultureInfo.InvariantCulture);

        _logger.Info($"Validation errors '{validationResult}'");
        
        return validationResult;
    }
}