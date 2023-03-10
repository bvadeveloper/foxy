using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Platform.Processor.Coordinator;

public static class Extensions
{
    internal static ProcessingTypes ToProcessingType(this string value) =>
        value.IsHostAddress() ? ProcessingTypes.Host :
        value.IsDomainName() ? ProcessingTypes.Domain :
        value.IsEmail() ? ProcessingTypes.Email :
        value.IsFacebookProfile() ? ProcessingTypes.Facebook : ProcessingTypes.None;

    private static bool IsHostAddress(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return IPAddress.TryParse(value, out _);
    }

    private static bool IsEmail(this string value)
    {
        var email = value;
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static bool IsFacebookProfile(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        // todo: need more details
        return Regex.IsMatch(value, "^[a-z ,.'-]+$", RegexOptions.Compiled);
    }

    private static bool IsDomainName(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;


        var split = value.ToLowerInvariant().Split('.');
        if (split.Length < 2)
        {
            return false;
        }

        var sld = split[0];
        var tld = string.Join(".", split.Skip(1));

        if (string.IsNullOrEmpty(tld) || string.IsNullOrEmpty(sld))
        {
            return false;
        }

        // alphanumeric, hyphen, each label less than 64 chars

        if (split.Any(l => string.IsNullOrEmpty(l)
                           || l.StartsWith("-")
                           || l.EndsWith("-")
                           || l.Length > 63
                           || (from c in l
                               where c != '-'
                               where c is < 'a' or > 'z'
                               where c is < '0' or > '9'
                               select c).Any()))
        {
            return false;
        }

        var dnsForm = value.ToDnsForm();
        if (dnsForm.StartsWith("xn--"))
        {
            try
            {
                new IdnMapping().GetUnicode(dnsForm);
            }
            catch (Exception)
            {
                return false;
            }
        }

        return true;
    }

    private static string ToDnsForm(this string input)
    {
        var split = input.Split('.')
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.ToLowerInvariant())
            .ToArray();

        return string.Join(".", split);
    }
}