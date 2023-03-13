using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Platform.Contract.Telegram;
using Platform.Logging.Extensions;

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

    public ValidationResult Validate(ITelegramMessage message)
    {
        var genericValidationType = typeof(IValidator<>).MakeGenericType(message.GetType());
        var methodInfo = genericValidationType.GetMethod("Validate");
        var validatorType = _types.FirstOrDefault(x =>
            genericValidationType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
        var instance = Activator.CreateInstance(validatorType);

        var validationResult = (ValidationResult)methodInfo.Invoke(instance, BindingFlags.Public, null,
            new[] { message }, CultureInfo.InvariantCulture);

        if (!validationResult.IsValid)
        {
            _logger.Warn($"Validation errors '{validationResult}'");
        }

        return validationResult;
    }
}