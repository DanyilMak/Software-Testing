namespace Lab1.Core;

public static class StringUtils
{
    public static string Capitalize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static string Reverse(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        return new string(input.Reverse().ToArray());
    }

    public static bool IsPalindrome(string input)
    {
        if (input == null)
            return false;

        var reversed = Reverse(input);
        return input.Equals(reversed, StringComparison.OrdinalIgnoreCase);
    }

    public static string Truncate(string input, int maxLength)
    {
        if (input == null)
            return null;

        if (input.Length <= maxLength)
            return input;

        return input.Substring(0, maxLength);
    }
}