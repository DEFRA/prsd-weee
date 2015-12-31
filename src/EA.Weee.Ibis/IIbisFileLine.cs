namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a line in an Ibis file.
    /// </summary>
    public interface IIbisFileLine
    {
        /// <summary>
        /// Returns the line type identifier of the file line.
        /// </summary>
        /// <returns></returns>
        string GetLineTypeIdentifier();

        /// <summary>
        /// Returns a list of data items that will be written to the file line.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetDataItems();
    }
}
