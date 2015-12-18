namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class WeeeDeliveredReturnVersion : Entity
    {
        public virtual ICollection<DataReturnVersion> DataReturnVersions { get; private set; }

        public virtual ICollection<AatfDeliveredAmount> AatfDeliveredAmounts { get; private set; }

        public virtual ICollection<AeDeliveredAmount> AeDeliveredAmounts { get; private set; }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected WeeeDeliveredReturnVersion()
        {
        }

        public WeeeDeliveredReturnVersion(DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            DataReturnVersions = new List<DataReturnVersion>();
            AatfDeliveredAmounts = new List<AatfDeliveredAmount>();
            AeDeliveredAmounts = new List<AeDeliveredAmount>();

            DataReturnVersions.Add(dataReturnVersion);
        }

        public void AddAatfDeliveredAmount(AatfDeliveredAmount aatfDeliveredAmount)
        {
            Guard.ArgumentNotNull(() => aatfDeliveredAmount, aatfDeliveredAmount);

            if (AatfDeliveredAmounts
                .Where(r => r.AatfDeliveryLocation.AatfApprovalNumber == aatfDeliveredAmount.AatfDeliveryLocation.AatfApprovalNumber)
                .Where(r => r.WeeeCategory == aatfDeliveredAmount.WeeeCategory)
                .Where(r => (r.ObligationType & aatfDeliveredAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            AatfDeliveredAmounts.Add(aatfDeliveredAmount);
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Parameter name is valid.")]
        public void AddAeDeliveredAmount(AeDeliveredAmount aeDeliveredAmount)
        {
            Guard.ArgumentNotNull(() => aeDeliveredAmount, aeDeliveredAmount);

            if (AeDeliveredAmounts
                .Where(r => r.AeDeliveryLocation.ApprovalNumber == aeDeliveredAmount.AeDeliveryLocation.ApprovalNumber)
                .Where(r => r.WeeeCategory == aeDeliveredAmount.WeeeCategory)
                .Where(r => (r.ObligationType & aeDeliveredAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            AeDeliveredAmounts.Add(aeDeliveredAmount);
        }
    }
}
