namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a type representing an error or a warning.
    /// </summary>
    public interface IErrorOrWarning
    {
        /// <summary>
        /// The severity of the error, for example "Error" or "Warning".
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// The human-readable text of the error or warning.
        /// </summary>
        string Description { get; }
    }
}
