using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentValidation;

namespace Platform.Validation.Fluent.Rules
{
    public static class DomainNameStaticValidator
    {
        public static void Validate<T>(string input, ValidationContext<T> context)
        {
            var validationContext = input.Validate();
            foreach (var (property, message) in validationContext)
                context.AddFailure(property, message);
        }

        private static Dictionary<string, string> Validate(this string input)
        {
            var validationContext = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(input))
            {
                validationContext.Add("domain name", "domain name is required");
                return validationContext;
            }

            var lowerCased = input.ToLowerInvariant();
            var split = lowerCased.Split('.');

            if (split.Length < 2)
            {
                validationContext.Add(input, "sld and tld should be provided");
                return validationContext;
            }

            var sld = split[0];
            var tld = string.Join(".", split.Skip(1));

            if (string.IsNullOrEmpty(tld))
            {
                validationContext.Add(input, "tld is required");
                return validationContext;
            }

            if (string.IsNullOrEmpty(sld))
            {
                validationContext.Add(input, "sld is required");
                return validationContext;
            }
            
            // alphanumeric, hyphen, each label less than 64 chars

            foreach (var l in split)
            {
                if (string.IsNullOrEmpty(l))
                {
                    validationContext.Add(input, "empty labels are not allowed");
                    return validationContext;
                }

                if (l.StartsWith("-"))
                {
                    validationContext.Add(input, "domain label can not start with hyphen");
                    return validationContext;
                }

                if (l.EndsWith("-"))
                {
                    validationContext.Add(input, "domain label can not end with hyphen");
                    return validationContext;
                }

                if (l.Length > 63)
                {
                    validationContext.Add(input, "each label should be less or equal 63 characters");
                    return validationContext;
                }

                foreach (var c in from c in l
                         where c != '-'
                         where c is < 'a' or > 'z'
                         where c is < '0' or > '9'
                         select c)
                {
                    validationContext.Add(input, $"invalid character in domain name: {c}");
                    return validationContext;
                }
            }

            var standardForm = input.DnsForm();
            if (standardForm.StartsWith("xn--"))
            {
                try
                {
                    var mapping = new IdnMapping();
                    mapping.GetUnicode(standardForm);
                }
                catch (Exception)
                {
                    validationContext.Add(input, "invalid idn name");
                }
            }

            return validationContext;
        }

        private static string DnsForm(this string input)
        {
            if (string.IsNullOrEmpty(input)) return null!;

            var split = input.Split('.')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLowerInvariant())
                .ToArray();

            return string.Join(".", split);
        }
    }
}