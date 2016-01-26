namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Prsd.Core;

    public class WeeeDeliveredReturnVersion : DataReturnVersionAssociativeEntity
    {
        public virtual ICollection<WeeeDeliveredAmount> WeeeDeliveredAmounts { get; private set; }

        public WeeeDeliveredReturnVersion()
        {
            WeeeDeliveredAmounts = new List<WeeeDeliveredAmount>();
        }

        public void AddWeeeDeliveredAmount(WeeeDeliveredAmount weeeDeliveredAmount)
        {
            Guard.ArgumentNotNull(() => weeeDeliveredAmount, weeeDeliveredAmount);

            if (weeeDeliveredAmount.IsAatfDeliveredAmount)
            {
                AddAatfDeliveredAmount(weeeDeliveredAmount);
            }
            else if (weeeDeliveredAmount.IsAeDeliveredAmount)
            {
                AddAeDeliveredAmount(weeeDeliveredAmount);
            }
        }

        private void AddAatfDeliveredAmount(WeeeDeliveredAmount aatfDeliveredAmount)
        {
            if (WeeeDeliveredAmounts
                .Where(r => r.IsAatfDeliveredAmount)
                .Where(r => r.AatfDeliveryLocation.ApprovalNumber == aatfDeliveredAmount.AatfDeliveryLocation.ApprovalNumber)
                .Where(r => r.WeeeCategory == aatfDeliveredAmount.WeeeCategory)
                .Where(r => (r.ObligationType & aatfDeliveredAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            WeeeDeliveredAmounts.Add(aatfDeliveredAmount);
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Parameter name aeDeliveredAmount is valid.")]
        private void AddAeDeliveredAmount(WeeeDeliveredAmount aeDeliveredAmount)
        {
            if (WeeeDeliveredAmounts
                .Where(r => r.IsAeDeliveredAmount)
                .Where(r => r.AeDeliveryLocation.ApprovalNumber == aeDeliveredAmount.AeDeliveryLocation.ApprovalNumber)
                .Where(r => r.WeeeCategory == aeDeliveredAmount.WeeeCategory)
                .Where(r => (r.ObligationType & aeDeliveredAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            WeeeDeliveredAmounts.Add(aeDeliveredAmount);
        }
    }
}
