using System.Text.RegularExpressions;
using Platform.Contract;
using FluentValidation;

namespace Platform.Validation.Fluent
{
    public class TargetModelValidator : AbstractValidator<TargetModel>
    {
        public TargetModelValidator()
        {
            RuleFor(model => model.Targets).NotNull().NotEmpty();
            RuleForEach(model => model.Targets).NotEmpty().Custom((text, context) =>
            {
                if (string.IsNullOrWhiteSpace(text)
                    || text.Length > 1024
                    || text.Contains('%')
                    || text.Contains('*')
                    || text.Contains('|')
                    || text.Contains('\\')
                    || text.Contains('#')
                    || text.Contains('&')
                    || text.Contains('^')
                    || text.Contains(')')
                    || text.Contains('(')
                    || text.Contains('!')
                    || text.Contains('+')
                    || text.Contains('~')
                    || text.Contains('"')
                    || text.Contains('`')
                    || text.Contains('$')
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