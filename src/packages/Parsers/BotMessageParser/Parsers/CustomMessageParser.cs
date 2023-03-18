using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.Contract.Profiles;
using Platform.Logging.Extensions;
using Platform.Validation.Fluent;
using Platform.Validation.Fluent.Messages;

namespace BotMessageParser.Parsers;

public class CustomMessageParser : IMessageParser
{
    private readonly IValidationFactory _validationFactory;
    private readonly ILogger _logger;
    private readonly StringBuilder _logHolder = new();

    public CustomMessageParser(IValidationFactory validationFactory, ILogger<CustomMessageParser> logger)
    {
        _validationFactory = validationFactory;
        _logger = logger;
    }

    public ValueTask<ParseResult> Parse(string input, CancellationToken cancellationToken)
    {
        _logHolder.Clear();

        var items = input.CustomSplit().ToImmutableArray();
        var command = items[0].ToLower();

        var hasOption = false;
        var option = string.Empty;
        var processingTypes = ProcessingTypes.None;
        var profiles = new List<CoordinatorProfile>();
        var checkHash = new HashSet<string>();

        switch (command)
        {
            case "scan":

                foreach (var arg in items.Skip(1))
                {
                    switch (arg)
                    {
                        case "--domains" or "-d":
                            processingTypes = ProcessingTypes.Domain;
                            continue;

                        case "--hosts" or "-h":
                            processingTypes = ProcessingTypes.Host;
                            continue;

                        case "--option" or "-o":
                            hasOption = true;
                            continue;

                        default:
                            if (hasOption)
                            {
                                option = arg;
                            }
                            else if (Validate(arg, processingTypes) && !checkHash.Contains(arg))
                            {
                                checkHash.Add(arg);
                                profiles.Add(CoordinatorProfile.Default(arg, processingTypes));
                            }

                            continue;
                    }
                }

                if (hasOption && true) // validate option string
                {
                    profiles = profiles.Select(p => p with { Options = option }).ToList();
                }

                break;

            case "search":

                foreach (var arg in items.Skip(1))
                {
                    switch (arg)
                    {
                        case "--emails" or "-e":
                            processingTypes = ProcessingTypes.Email;
                            continue;

                        case "--bitcoins" or "-b":
                            processingTypes = ProcessingTypes.Bitcoin;
                            continue;

                        case "--option" or "-o":
                            hasOption = true;
                            continue;

                        default:
                            if (hasOption)
                            {
                                option = arg;
                            }
                            else if (Validate(arg, processingTypes) && !checkHash.Contains(arg))
                            {
                                profiles.Add(CoordinatorProfile.Default(arg, processingTypes));
                            }

                            continue;
                    }
                }

                if (hasOption && true) // validate option string
                {
                    profiles = profiles.Select(p => p with { Options = option }).ToList();
                }

                break;

            case "parse":

                foreach (var arg in items.Skip(1))
                {
                    switch (arg)
                    {
                        case "--facebook" or "-f":
                            processingTypes = ProcessingTypes.Facebook;
                            continue;

                        case "--instagram" or "-i":
                            processingTypes = ProcessingTypes.Instagram;
                            continue;

                        case "--option" or "-o":
                            hasOption = true;
                            continue;

                        default:
                            if (hasOption)
                            {
                                option = arg;
                            }
                            else if (Validate(arg, processingTypes) && !checkHash.Contains(arg))
                            {
                                profiles.Add(CoordinatorProfile.Default(arg, processingTypes));
                            }

                            continue;
                    }
                }

                if (hasOption && true) // validate option string
                {
                    profiles = profiles.Select(p => p with { Options = option }).ToList();
                }

                break;

            case "--help" or "-h":
                _logHolder.Append(CommandMessages.RootCommandDescription);
                break;

            default:
                _logHolder.Append(CommandMessages.ParseDefaultMessage);
                break;
        }

        return new ValueTask<ParseResult>(new ParseResult(profiles.Any(), profiles.ToImmutableArray(), _logHolder.ToString()));
    }

    private bool Validate(string value, ProcessingTypes processingTypes)
    {
        try
        {
            var validationResult = _validationFactory.Validate(ValidationMessageFactory(value, processingTypes));
            _logHolder.Append(validationResult);
            return validationResult.IsValid;
        }
        catch (Exception e)
        {
            _logHolder.Append("Looks like we caught the problem in the validation flow.");
            _logger.Error($"Validation error '{e.Message}'", e);
            return false;
        }
    }

    private static ITelegramValidationMessage ValidationMessageFactory(string value, ProcessingTypes type) =>
        type switch
        {
            ProcessingTypes.Domain => new DomainValidationMessage(new[] { value }),
            ProcessingTypes.Host => new HostValidationMessage(new[] { value }),
            ProcessingTypes.Email => new EmailValidationMessage(value),
            ProcessingTypes.Facebook => new FacebookValidationMessage(value),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Can't create validation message by processing type '{type}'")
        };
}