using System.Text.RegularExpressions;
using FluentValidation;
using Platform.Contract;

namespace Platform.Validation.Fluent.Rules
{
    public class IpTargetValidator : AbstractValidator<IpTarget>
    {
        public IpTargetValidator()
        {
            RuleFor(model => model.Value)
                .NotNull()
                .NotEmpty()
                .Custom((text, context) =>
                {
                    if (string.IsNullOrWhiteSpace(text)
                        || text.Length > 1024
                        || IsPrivateIp(text))
                    {
                        context.AddFailure("not valid symbols caught");
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