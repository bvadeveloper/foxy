using System;
using System.CommandLine;
using System.Net;
using Platform.Contract.Telegram;
using Platform.Validation.Fluent.Rules;

namespace Platform.Telegram.Bot.Parser;

internal static class MessageParser
{
    private static readonly Option<Uri[]> DomainOption;
    private static readonly Option<IPAddress[]> HostOption;
    private static readonly Option<string[]> EmailOption;
    private static readonly Option<string[]> FacebookOption;
    private static readonly Option<string> ExtraOption;

    static MessageParser()
    {
        DomainOption = new Option<Uri[]>(new[] { "--domain", "-d" })
        {
            Description = "Domains scan",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore,
        };

        DomainOption.AddValidator(result =>
        {
            try
            {
                var domains = result.GetValueForOption(DomainOption);
                var validate = new DomainValidator().Validate(new DomainMessage(domains));
                if (!validate.IsValid)
                {
                    result.ErrorMessage = validate.ToString();
                }
            }
            catch (InvalidOperationException e)
            {
                result.ErrorMessage = e.Message;
            }
        });

        HostOption = new Option<IPAddress[]>(new[] { "--host", "-h" })
        {
            Description = "Hosts scan",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        HostOption.AddValidator(result =>
        {
            try
            {
                var hosts = result.GetValueForOption(HostOption);
                var validate = new HostValidator().Validate(new HostMessage(hosts));
                if (!validate.IsValid)
                {
                    result.ErrorMessage = validate.ToString();
                }
            }
            catch (InvalidOperationException e)
            {
                result.ErrorMessage = e.Message;
            }
        });

        EmailOption = new Option<string[]>(new[] { "--email", "-e" })
        {
            Description = "Check emails",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        FacebookOption = new Option<string[]>(new[] { "--facebook", "-f" })
        {
            Description = "Parse profile",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore
        };

        ExtraOption = new Option<string>(new[] { "--options", "-o" })
        {
            Description = "Additional arguments for command (optional in some special cases).",
            IsRequired = false,
            IsHidden = true
        };
    }

    internal static RootCommand MakeCommand(Action<IPAddress[], Uri[], string[], string[], string> handle)
    {
        var command = new RootCommand("Simple Foxy CLI for Telegram bot.");
        command.AddOption(HostOption);
        command.AddOption(DomainOption);
        command.AddOption(EmailOption);
        command.AddOption(FacebookOption);
        command.AddOption(ExtraOption);

        command.SetHandler(handle, HostOption, DomainOption, EmailOption, FacebookOption, ExtraOption);

        return command;
    }
}