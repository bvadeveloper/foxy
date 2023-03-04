using System;
using System.Globalization;
using System.Linq;
using System.Net;
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

    public ValidationResult Validate(string message)
    {
        var request = MakeValidationMessage(message);
        var genericValidationType = typeof(IValidator<>).MakeGenericType(request.GetType());
        var methodInfo = genericValidationType.GetMethod("Validate");
        var validatorType = _types.FirstOrDefault(x =>
            genericValidationType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
        var instance = Activator.CreateInstance(validatorType);

        var validationResult = (ValidationResult)methodInfo.Invoke(instance, BindingFlags.Public, null,
            new[] { request }, CultureInfo.InvariantCulture);

        if (!validationResult.IsValid)
        {
            _logger.Warn($"Validation errors '{validationResult}'");
        }

        return validationResult;
    }

    private static IValidationMessage MakeValidationMessage(string message) =>
        IPAddress.TryParse(message, out var ipAddress)
            ? new IpMessage(Name: ipAddress.ToString())
            : new DomainMessage(Name: message);
}