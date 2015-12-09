namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

    public class DataReturn : Entity
    {
        public virtual Scheme Scheme { get; private set; }

        public virtual int ComplianceYear { get; private set; }

        public virtual int Quarter { get; private set; }

        public DataReturn(Scheme scheme, int complianceYear, int quarter)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            Scheme = scheme;
            Quarter = quarter;
            ComplianceYear = complianceYear;            
        }
        protected DataReturn()
        {
        }

        public virtual DataReturnVersion CurrentDataReturnVersion { get; private set; }
       
        public void SetCurrentDataReturnVersion(DataReturnVersion returnVersion)
        {
            Guard.ArgumentNotNull(() => returnVersion, returnVersion);

            if (returnVersion.DataReturn != this)
            {
                throw new InvalidOperationException();
            }
            CurrentDataReturnVersion = returnVersion;
        }
    }
}