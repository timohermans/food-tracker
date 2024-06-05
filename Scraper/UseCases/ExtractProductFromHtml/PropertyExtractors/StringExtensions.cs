using System.Text;

namespace Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;

public static class StringExtensions
{
    public static string RemoveSpecialCharacters(this string value)
    {
        char[] otherSpecials = [',', '%', ' ', '[', ']', '(', ')', '-', '\''];
        StringBuilder sb = new StringBuilder();
        foreach (char c in value)
        {
            if ((c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') ||
                (c >= 'a' && c <= 'z') ||
                (c >= 'À' && c <= 'ÿ') ||
                otherSpecials.Contains(c))
            {
                sb.Append(c);
            }

            if (c == '\u00A0')
            {
                sb.Append(" ");
            }
        }
        return sb.ToString();
    }
}