using System.Text.RegularExpressions;

namespace TestAssignment.Services;

public partial class EmailExtractor
{
    [GeneratedRegex(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}", RegexOptions.IgnoreCase)]
    private static partial Regex SearchEmailRegex();

    /// <summary>
    /// Метод получает все email адреса из текста.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Список найденных email-адресов.</returns>
    public static List<string> Extract(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var result = new List<string>();

        var matches = SearchEmailRegex().EnumerateMatches(text);

        foreach (var match in matches)
        {
            ReadOnlySpan<char> emailSpan = text.AsSpan(match.Index, match.Length);
            result.Add(emailSpan.ToString());
        }

        return result;
    }
}