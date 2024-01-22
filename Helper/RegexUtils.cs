using System.Text.RegularExpressions;

namespace OnlineMarket.Helper
{
    public class RegexUtils
    {
        public static readonly Regex SlugRegex = new Regex(@"(^[a-z0-9])([a-z0-9_-]+)*([a-z0-9])$");
    }
}
