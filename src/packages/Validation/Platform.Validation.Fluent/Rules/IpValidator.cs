using System.Text.RegularExpressions;
using FluentValidation;
using Platform.Validation.Fluent.Messages;

namespace Platform.Validation.Fluent.Rules
{
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

        private static bool IsPrivateIp(string text) =>
            Regex.IsMatch(text,
                "(^127\\.)" +
                "|(^10\\.)" +
                "|(^172\\.1[6-9]\\.)" +
                "|(^172\\.2[0-9]\\.)" +
                "|(^172\\.3[0-1]\\.)" +
                "|(^192\\.168\\.)" +
                "|localhost" +
                "|255.255.255.255" +
                "|(^0\\.)" +
                "|other_stuff_here/gm",
                RegexOptions.Compiled);
    }
}