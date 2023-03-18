using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Platform.Contract.Profiles;
using Platform.Validation.Fluent;
using Platform.Validation.Fluent.Messages;

namespace BotMessageParser.Parsers;

/// <summary>
///  Yep, I know it's ugly, but it works, waiting for new release of System.CommandLine
/// https://github.com/dotnet/command-line-api
/// </summary>
public class CommandLineParser : IMessageParser
{
    private readonly IValidationFactory _validationFactory;

    private readonly Option<string[]> _domainOption;
    private readonly Option<string[]> _hostOption;
    private readonly Option<string[]> _emailOption;
    private readonly Option<string[]> _bitcoinOption;
    private readonly Option<string[]> _facebookOption;
    private readonly Option<string[]> _instagramOption;
    private readonly Option<string> _extraOption;

    private readonly Command _scanCommand;
    private readonly Command _searchCommand;
    private readonly Command _parseCommand;

    public CommandLineParser(IValidationFactory validationFactory)
    {
        _validationFactory = validationFactory;

        #region Scan

        _domainOption = new Option<string[]>(
            aliases: new[] { "--domains", "-d" },
            description: CommandMessages.DomainOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        _hostOption = new Option<string[]>(
            aliases: new[] { "--ips", "-i" },
            description: CommandMessages.HostOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        #endregion

        #region Search section

        _emailOption = new Option<string[]>(
            aliases: new[] { "--emails", "-e" },
            description: CommandMessages.EmailOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        _bitcoinOption = new Option<string[]>(
            aliases: new[] { "--bitcoins", "-b" },
            description: CommandMessages.BitcoinOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        #endregion

        #region Social media

        _facebookOption = new Option<string[]>(
            aliases: new[] { "--facebook", "-f" },
            description: CommandMessages.FacebookOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        _instagramOption = new Option<string[]>(
            aliases: new[] { "--instagram", "-i" },
            description: CommandMessages.InstagramOptionDescription)
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        #endregion

        #region Extra options

        _extraOption = new Option<string>(
            aliases: new[] { "--options", "-o" },
            description: CommandMessages.ExtraOptionsOptionDescription)
        {
            IsRequired = false,
            IsHidden = true
        };

        #endregion

        #region Commands

        _scanCommand = new Command("scan", CommandMessages.ScanCommandDescription)
        {
            _domainOption,
            _hostOption,
        };

        _searchCommand = new Command("search", CommandMessages.SearchCommandDescription)
        {
            _emailOption,
            _bitcoinOption,
        };

        _parseCommand = new Command("parse", CommandMessages.ParseCommandDescription)
        {
            _facebookOption,
            _instagramOption,
        };

        #endregion
    }

    public async ValueTask<ParseResult> Parse(string input, CancellationToken cancellationToken)
    {
        _domainOption.AddValidator(context => Validate<DomainValidationMessage>(context, _domainOption));
        _hostOption.AddValidator(context => Validate<HostValidationMessage>(context, _hostOption));

        var profiles = new List<CoordinatorProfile>();
        var rootCommand = new RootCommand(CommandMessages.RootCommandDescription);
        rootCommand.AddCommand(_scanCommand);
        rootCommand.AddCommand(_searchCommand);
        rootCommand.AddCommand(_parseCommand);
        rootCommand.AddGlobalOption(_extraOption);

        _scanCommand.SetHandler((domains, hosts, options) =>
        {
            if (domains.IsAny())
                profiles.AddRange(domains.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Domain, options)));

            if (hosts.IsAny())
                profiles.AddRange(hosts.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Host, options)));
        }, _domainOption, _hostOption, _extraOption);


        _searchCommand.SetHandler((emails, bitcoins, options) =>
        {
            if (emails.IsAny())
                profiles.AddRange(emails.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Email, options)));

            if (bitcoins.IsAny())
                profiles.AddRange(bitcoins.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Bitcoin, options)));
        }, _emailOption, _bitcoinOption, _extraOption);


        _parseCommand.SetHandler((facebook, instagram, options) =>
        {
            if (facebook.IsAny())
                profiles.AddRange(facebook.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Facebook, options)));

            if (instagram.IsAny())
                profiles.AddRange(instagram.Select(value => CoordinatorProfile.Make(value, ProcessingTypes.Instagram, options)));
        }, _facebookOption, _instagramOption, _extraOption);


        var logger = new ParserLogger();
        var resultCode = await rootCommand.InvokeAsync(input, logger);

        return new ParseResult(resultCode == 0 && profiles.Any(), profiles.ToImmutableArray(), logger.CollectOutput());
    }

    private void Validate<T>(OptionResult context, Option<string[]> option) where T : ITelegramValidationMessage
    {
        try
        {
            var value = context.GetValueForOption(option);
            var validationModel = (T)Activator.CreateInstance(typeof(T), new object[] { value });
            var validationResult = _validationFactory.Validate(validationModel);
            context.ErrorMessage = validationResult.IsValid switch
            {
                false => validationResult.ToString(),
                _ => context.ErrorMessage
            };
        }
        catch (InvalidOperationException e)
        {
            context.ErrorMessage = e.Message;
        }
    }
}