namespace EA.Weee.Core.Shared
{
    /// <summary>
    /// Provides methods for sanitizing strings that may pose a threat to Excel.
    /// </summary>
    public interface IExcelSanitizer
    {
        /// <summary>
        /// Tests if the specified input is considered a threat to Excel.
        /// </summary>
        /// <param name="input">The string to test.</param>
        /// <returns>Returns true if the input is a threat.</returns>
        bool IsThreat(string input);

        /// <summary>
        /// Returns a sanitized copy of the specified input.
        /// </summary>
        /// <param name="input">The string to sanitize.</param>
        /// <returns>An Excel-friendly copy of the input.</returns>
        string Sanitize(string input);
    }
}
