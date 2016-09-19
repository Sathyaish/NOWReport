using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Wintellect.Extensions
{
    public static class StringExtensions
    {
        private static Regex PascalCaseRegEx = new Regex("([A-Z]+[a-z]+)");

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static string Plus(this string s, params string[] args)
        {
            return s.Plus(new StringBuilder(), args);
        }

        public static string Plus(this string s, StringBuilder builder, params string[] args)
        {
            if (args.IsNull() || args.ThereAreNone()) return null;

            if (!s.IsNull()) builder.Append(s);

            args.ForEach(arg => builder.Append(arg));

            return builder.ToString();
        }

        public static bool IsValidEmail(this string s)
        {
            try
            {
                var mailAddress = new MailAddress(s);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ToTitleCase(this string s)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(s);
        }

        public static string ToTitleCase(this string s, char oldChar, char newChar)
        {
            return s.Replace(oldChar, newChar).ToTitleCase();
        }

        public static string ToTitleCase(this string s, string oldValue, string newValue)
        {
            return s.Replace(oldValue, newValue).ToTitleCase();
        }

        public static string FromPascalToTitleCase(this string s)
        {
            return PascalCaseRegEx.Replace(s, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()).Plus(" "));
        }
    }
}