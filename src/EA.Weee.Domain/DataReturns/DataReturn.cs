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
    /// The CurrentVersion property provides a link to the current version of the return.
    /// </summary>
    public class DataReturn : Entity
    {
        public virtual Scheme.Scheme Scheme { get; private set; }

        public virtual Quarter Quarter { get; private set; }

        /// <summary>
        /// Provides the current version of the data return.
        /// To replace the version, use the SetCurrentVersion method.
        /// </summary>
        public virtual DataReturnVersion CurrentVersion { get; private set; }

        public DataReturn(Scheme.Scheme scheme, Quarter quarter)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => quarter, quarter);

            Scheme = scheme;
            Quarter = quarter;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected DataReturn()
        {
        }

        public void SetCurrentVersion(DataReturnVersion version)
        {
            Guard.ArgumentNotNull(() => version, version);

            if (version.DataReturn != this)
            {
                string errorMessage = "The specified data return version does not relate to this data return.";
                throw new InvalidOperationException(errorMessage);
            }

            CurrentVersion = version;
        }
    }
}
