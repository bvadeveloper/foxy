using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using FluentValidation;
using Platform.Validation.Fluent.Messages;

namespace Platform.Validation.Fluent.Rules;

public class HostValidator : AbstractValidator<HostValidationMessage>
{
    public HostValidator()
    {
        RuleForEach(model => model.Value)
            .NotNull()
            .NotEmpty()
            .Custom((ipAddress, context) =>
            {
                if (string.IsNullOrWhiteSpace(ipAddress)
                    || ipAddress.Length > 1024
                    || IsPrivateIp(ipAddress))
                {
                    context.AddFailure($"Hmm... I see not allowed IP address: {ipAddress}");
                }
            });
    }

    private static bool IsPrivateIp(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (IPAddress.TryParse(text, out var address))
        {
            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                {
                    // IPv4
                    var ipv4Regex = new Regex(
                        "(^127\\.)" +
                        "|(^10\\.)" +
                        "|(^172\\.1[6-9]\\.)" +
                        "|(^172\\.2[0-9]\\.)" +
                        "|(^172\\.3[0-1]\\.)" +
                        "|(^192\\.168\\.)" +
                        "|(^169\\.254\\.)" +
                        "|(^100\\.(6[4-9]|[7-9]\\d|1[01]\\d|12[0-7])\\.)" +
                        "|(^192\\.0\\.0\\.)" +
                        "|(^198\\.1[8-9]\\.)" +
                        "|(^198\\.51\\.100\\.)" +
                        "|(^203\\.0\\.113\\.)" +
                        "|(^224\\.)" +
                        "|(^240\\.)" +
                        "|(^0\\.)" +
                        "|(^255\\.255\\.255\\.255$)",
                        RegexOptions.Compiled);

                    return ipv4Regex.IsMatch(text);
                }
                case AddressFamily.InterNetworkV6:
                {
                    // IPv6
                    var ipv6Regex = new Regex(
                        "^([:fF]{4}(?::[:fF]{4})*)?" + // Matches the optional prefix
                        "(::([:fF]{4}(?::[:fF]{4})*)?)?" + // Matches the optional '::' separator and suffix
                        "(([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4})" + // Matches a full IPv6 address with 8 blocks
                        "|(([0-9A-Fa-f]{1,4}:)*[0-9A-Fa-f]{1,4})?" + // Matches a compressed IPv6 address
                        "$",
                        RegexOptions.Compiled);

                    return ipv6Regex.IsMatch(text);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return false;
    }
}