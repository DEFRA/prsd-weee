namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core;

    public class WeeeCollectedReturnVersion : DataReturnVersionAssociativeEntity
    {
        public virtual ICollection<WeeeCollectedAmount> WeeeCollectedAmounts { get; private set; }

        public WeeeCollectedReturnVersion()
        {
            WeeeCollectedAmounts = new List<WeeeCollectedAmount>();
        }

        public void AddWeeeCollectedAmount(WeeeCollectedAmount weeeCollectedAmount)
        {
            Guard.ArgumentNotNull(() => weeeCollectedAmount, weeeCollectedAmount);
            switch (weeeCollectedAmount.SourceType)
            {
                case WeeeCollectedAmountSourceType.Dcf:
                    AddReturnItemCollectedFromDcf(weeeCollectedAmount);
                    break;
                case WeeeCollectedAmountSourceType.Distributor:
                    AddB2cWeeeFromDistributor(weeeCollectedAmount);
                    break;
                case WeeeCollectedAmountSourceType.FinalHolder:
                    AddB2cWeeeFromFinalHolder(weeeCollectedAmount);
                    break;
            }
        }

        private void AddReturnItemCollectedFromDcf(WeeeCollectedAmount weeeCollectedAmount)
        {
            Guard.ArgumentNotNull(() => weeeCollectedAmount, weeeCollectedAmount);

            if (WeeeCollectedAmounts
                .Where(r => r.SourceType == WeeeCollectedAmountSourceType.Dcf)
                .Where(r => r.WeeeCategory == weeeCollectedAmount.WeeeCategory)
                .Where(r => (r.ObligationType & weeeCollectedAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            WeeeCollectedAmounts.Add(weeeCollectedAmount);
        }

        private void AddB2cWeeeFromDistributor(WeeeCollectedAmount weeeCollectedAmount)
        {
            Guard.ArgumentNotNull(() => weeeCollectedAmount, weeeCollectedAmount);

            if (weeeCollectedAmount.ObligationType != ObligationType.B2C)
            {
                string errorMessage = "Only return items with an obligation type of B2C can be added under \"B2C WEEE from distributors\".";
                throw new InvalidOperationException(errorMessage);
            }

            if (WeeeCollectedAmounts
                .Where(r => r.SourceType == WeeeCollectedAmountSourceType.Distributor)
                .Where(r => r.WeeeCategory == weeeCollectedAmount.WeeeCategory)
                .Where(r => (r.ObligationType & weeeCollectedAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            WeeeCollectedAmounts.Add(weeeCollectedAmount);
        }

        private void AddB2cWeeeFromFinalHolder(WeeeCollectedAmount weeeCollectedAmount)
        {
            Guard.ArgumentNotNull(() => weeeCollectedAmount, weeeCollectedAmount);

            if (weeeCollectedAmount.ObligationType != ObligationType.B2C)
            {
                string errorMessage = "Only return items with an obligation type of B2C can be added under \"B2C WEEE from final holders\".";
                throw new InvalidOperationException(errorMessage);
            }

            if (WeeeCollectedAmounts
                .Where(r => r.SourceType == WeeeCollectedAmountSourceType.FinalHolder)
                .Where(r => r.WeeeCategory == weeeCollectedAmount.WeeeCategory)
                .Where(r => (r.ObligationType & weeeCollectedAmount.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            WeeeCollectedAmounts.Add(weeeCollectedAmount);
        }
    }
}
