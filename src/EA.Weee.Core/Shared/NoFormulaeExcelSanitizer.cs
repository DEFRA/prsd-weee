namespace EA.Weee.Core.Shared
{
    using System;

    /// <summary>
    /// This implementation of <see cref="IExcelSanitizer"/> removes all leading equals signs
    /// to prevent any Excel interpreting anything as a formula.
    /// </summary>
    public class NoFormulaeExcelSanitizer : IExcelSanitizer
    {
        public bool IsThreat(string input)
        {
            return input.StartsWith("=");
        }

        public string Sanitize(string input)
        {
            if (!IsThreat(input))
            {
                return input;
            }
            else
            {
                string sanitizedInput = input.TrimStart('=');

                // Check that the input really has been sanitized.
                if (IsThreat(sanitizedInput))
                {
                    string message = string.Format(
                        "Failed to correctly sanitize \"{0}\".",
                        input);
                    throw new Exception(message);
                }

                return sanitizedInput;
            }
        }
    }
}
