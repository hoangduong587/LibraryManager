using System.Text.RegularExpressions;

namespace LibraryManager.Api.Helpers
{
    public static class ValidationInputHelper
    {
        // Allows only letters and numbers (A-Z, a-z, 0-9)
        private static readonly Regex _alphaNumericRegex =
            new Regex("^[a-zA-Z0-9]+$", RegexOptions.Compiled);

        public static bool IsValidName(string? input)
        {
            // Must not be null, empty, or whitespace
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Must be alphanumeric only
            return _alphaNumericRegex.IsMatch(input);
        }
    }
}
