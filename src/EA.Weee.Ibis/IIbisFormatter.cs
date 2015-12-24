namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An IIbisFormatter is capable of sanitizing and encoding string data such
    /// that it can be written to an Ibis file.
    /// </summary>
    public interface IIbisFormatter
    {
        /// <summary>
        /// Sanitizes and encodes the data such that it can be written to an Ibis file.
        /// </summary>
        /// <param name="input">The input to be sanitized and encoded.</param>
        /// <returns>Returns a string that can be directly written to a Ibis file.</returns>
        string Format(string input);
    }
}
