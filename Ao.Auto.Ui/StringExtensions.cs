using System.Text.RegularExpressions;

namespace Ao.Auto.Ui
{
    public static class StringExtensions
    {
        private static readonly Regex Whitespace = new Regex(@"\s+");
        
        public static string WithoutWhitespace(this string str) =>
            Whitespace.Replace(str, string.Empty);
    }
}
