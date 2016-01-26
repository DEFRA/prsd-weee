namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class DataReturnVersionAssociativeEntity : Entity
    {
        public virtual ICollection<DataReturnVersion> DataReturnVersions { get; private set; }

        public DataReturnVersionAssociativeEntity()
        {
            DataReturnVersions = new List<DataReturnVersion>();
        }

        internal virtual void AddDataReturnVersion(DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            var existingDataReturnVersion = DataReturnVersions.FirstOrDefault();
            if (existingDataReturnVersion != null &&
                dataReturnVersion.DataReturn != existingDataReturnVersion.DataReturn)
            {
                throw new InvalidOperationException("The data return version does not belong to the current data return");
            }

            DataReturnVersions.Add(dataReturnVersion);
        }
    }
}
