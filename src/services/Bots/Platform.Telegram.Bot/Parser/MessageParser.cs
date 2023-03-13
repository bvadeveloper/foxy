using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Platform.Contract.Profiles;
using Platform.Contract.Telegram;
using Platform.Validation.Fluent;

namespace Platform.Telegram.Bot.Parser;

internal class MessageParser : IMessageParser
{
    private const string RootCommandDescription = "Simple Foxy CLI for Telegram";
    private readonly IValidationFactory _validationFactory;

    private readonly Option<string[]> _domainOption;
    private readonly Option<string[]> _hostOption;
    private readonly Option<string[]> _emailOption;
    private readonly Option<string[]> _facebookOption;
    private readonly Option<string> _extraOption;

    public MessageParser(IValidationFactory validationFactory)
    {
        _validationFactory = validationFactory;

        _domainOption = new Option<string[]>(new[] { "--domain", "-d" })
        {
            Description = "Domain scan",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore,
        };


        _hostOption = new Option<string[]>(new[] { "--host", "-h" })
        {
            Description = "Host scan",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };


        _emailOption = new Option<string[]>(new[] { "--email", "-e" })
        {
            Description = "Check emails",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        _facebookOption = new Option<string[]>(new[] { "--facebook", "-f" })
        {
            Description = "Parse profile",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        _extraOption = new Option<string>(new[] { "--options", "-o" })
        {
            Description = "Additional arguments for command (optional in some special cases).",
            IsRequired = false,
            IsHidden = true
        };
    }
    
    public async Task<ParseResult> Parse(string input)
    {
        var profiles = new List<CoordinatorProfile>();

        var rootCommand = Init((hosts, uris, emails, facebook, options) =>
        {
            profiles.Add(CoordinatorProfile.Make(hosts, ProcessingTypes.Host, options));
            profiles.Add(CoordinatorProfile.Make(uris, ProcessingTypes.Domain, options));
            profiles.Add(CoordinatorProfile.Make(emails, ProcessingTypes.Email, options));
            profiles.Add(CoordinatorProfile.Make(facebook, ProcessingTypes.Facebook, options));
        });

        var logger = new ParserLogger();
        var resultCode = await rootCommand.InvokeAsync(input, logger);
        var logOutput = $"{logger.Out}{logger.Error}";

        return new ParseResult(resultCode == 0 && profiles.Any(), profiles.ToImmutableList(), logOutput);
    }

    private RootCommand Init(Action<string[], string[], string[], string[], string> handle)
    {
        AddValidators();

        var rootCommand = new RootCommand(RootCommandDescription);
        rootCommand.AddOption(_hostOption);
        rootCommand.AddOption(_domainOption);
        rootCommand.AddOption(_emailOption);
        rootCommand.AddOption(_facebookOption);
        rootCommand.AddOption(_extraOption);

        rootCommand.SetHandler(handle, _hostOption, _domainOption, _emailOption, _facebookOption, _extraOption);

        return rootCommand;
    }

    private void AddValidators()
    {
        _domainOption.AddValidator(context =>
        {
            try
            {
                var value = context.GetValueForOption(_domainOption);
                var validationResult = _validationFactory.Validate(new DomainMessage(value));
                if (!validationResult.IsValid)
                {
                    context.ErrorMessage = validationResult.ToString();
                }
            }
            catch (InvalidOperationException e)
            {
                context.ErrorMessage = e.Message;
            }
        });

        _hostOption.AddValidator(context =>
        {
            try
            {
                var value = context.GetValueForOption(_hostOption);
                var validationResult = _validationFactory.Validate(new HostMessage(value));
                if (!validationResult.IsValid)
                {
                    context.ErrorMessage = validationResult.ToString();
                }
            }
            catch (InvalidOperationException e)
            {
                context.ErrorMessage = e.Message;
            }
        });
    }
}