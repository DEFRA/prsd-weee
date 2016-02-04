namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core;

    public class EeeOutputReturnVersion : DataReturnVersionAssociativeEntity
    {
        public virtual ICollection<EeeOutputAmount> EeeOutputAmounts { get; private set; }

        public EeeOutputReturnVersion()
        {
            EeeOutputAmounts = new List<EeeOutputAmount>();
        }

        internal override void AddDataReturnVersion(DataReturnVersion dataReturnVersion)
        {
            var eeeOutputAmount = EeeOutputAmounts.FirstOrDefault();
            if (eeeOutputAmount != null &&
                dataReturnVersion != null)
            {
                ValidateEeeOutputAmountAndDataReturnVersion(eeeOutputAmount, dataReturnVersion);
            }

            base.AddDataReturnVersion(dataReturnVersion);
        }

        public void AddEeeOutputAmount(EeeOutputAmount eeeOutputAmount)
        {
            Guard.ArgumentNotNull(() => eeeOutputAmount, eeeOutputAmount);

            var dataReturnVersion = DataReturnVersions.FirstOrDefault();
            if (dataReturnVersion != null)
            {
                ValidateEeeOutputAmountAndDataReturnVersion(eeeOutputAmount, dataReturnVersion);
            }

            EeeOutputAmounts.Add(eeeOutputAmount);
        }

        private static void ValidateEeeOutputAmountAndDataReturnVersion(EeeOutputAmount eeeOutputAmount, DataReturnVersion dataReturnVersion)
        {
            if (eeeOutputAmount.RegisteredProducer.Scheme.ApprovalNumber != dataReturnVersion.DataReturn.Scheme.ApprovalNumber)
            {
                string errorMesage = "The producer for the EEE output amount and the data return do not belong to the same scheme.";
                throw new InvalidOperationException(errorMesage);
            }

            if (eeeOutputAmount.RegisteredProducer.ComplianceYear != dataReturnVersion.DataReturn.Quarter.Year)
            {
                string errorMesage = "The producer for the EEE output amount and the data return do not have a matching compliance year.";
                throw new InvalidOperationException(errorMesage);
            }
        }
    }
}
