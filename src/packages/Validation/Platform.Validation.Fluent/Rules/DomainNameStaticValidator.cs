using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentValidation;

namespace Platform.Validation.Fluent.Rules
{
    public static class DomainNameStaticValidator
    {
        public static void Validate<T>(Uri uri, ValidationContext<T> context)
        {
            foreach (var (property, message) in uri.Validate())
            {
                context.AddFailure(property, message);
            }
        }

        private static Dictionary<string, string> Validate(this Uri input)
        {
            var value = input.ToString();
            var context = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(value))
            {
                context.Add("domain name", "domain name is required");
                return context;
            }

            var lowerCased = value.ToLowerInvariant();
            var split = lowerCased.Split('.');

            if (split.Length < 2)
            {
                context.Add(value, "sld and tld should be provided");
                return context;
            }

            var sld = split[0];
            var tld = string.Join(".", split.Skip(1));

            if (string.IsNullOrEmpty(tld))
            {
                context.Add(value, "tld is required");
                return context;
            }

            if (string.IsNullOrEmpty(sld))
            {
                context.Add(value, "sld is required");
                return context;
            }

            // alphanumeric, hyphen, each label less than 64 chars

            foreach (var l in split)
            {
                if (string.IsNullOrEmpty(l))
                {
                    context.Add(value, "empty labels are not allowed");
                    return context;
                }

                if (l.StartsWith("-"))
                {
                    context.Add(value, "domain label can not start with hyphen");
                    return context;
                }

                if (l.EndsWith("-"))
                {
                    context.Add(value, "domain label can not end with hyphen");
                    return context;
                }

                if (l.Length > 63)
                {
                    context.Add(value, "each label should be less or equal 63 characters");
                    return context;
                }

                foreach (var c in from c in l
                         where c != '-'
                         where c is < 'a' or > 'z'
                         where c is < '0' or > '9'
                         select c)
                {
                    context.Add(value, $"invalid character in domain name: {c}");
                    return context;
                }
            }

            var standardForm = value.DnsForm();
            if (standardForm.StartsWith("xn--"))
            {
                try
                {
                    var mapping = new IdnMapping();
                    mapping.GetUnicode(standardForm);
                }
                catch (Exception)
                {
                    context.Add(value, "invalid idn name");
                }
            }

            return context;
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