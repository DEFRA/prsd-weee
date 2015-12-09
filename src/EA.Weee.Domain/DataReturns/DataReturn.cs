namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    /// <summary>
    /// This entity represents a scheme's data return for a single quarter.
    /// The Contents property provides a link to the current contents of the return.
    /// </summary>
    public class DataReturn : Entity
    {
        public Scheme.Scheme Scheme { get; private set; }

        public Quarter Quarter { get; private set; }

        /// <summary>
        /// Provides the current contents of the data return.
        /// To replace the contents, use teh SetContents method.
        /// </summary>
        public DataReturnContents Contents { get; private set; }

        public DataReturn(Scheme.Scheme scheme, Quarter quarter)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => quarter, quarter);

            Scheme = scheme;
            Quarter = quarter;
        }

        public void SetContents(DataReturnContents contents)
        {
            Guard.ArgumentNotNull(() => contents, contents);

            if (contents.DataReturn != this)
            {
                string errorMessage = "The specified data return contents does not relate to this data return.";
                throw new InvalidOperationException(errorMessage);
            }

            Contents = contents;
        }
    }
}
