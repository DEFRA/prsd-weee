﻿namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    /// <summary>
    /// This entity provides the content for a data return. Each data return may have
    /// any number of versions of the contents, but only one will be the "current" version.
    /// </summary>
    public class DataReturnVersion : Entity
    {
        public virtual DataReturn DataReturn { get; private set; }

        public virtual DateTime? SubmittedDate { get; private set; }

        public string SubmittingUserId { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual ICollection<WeeeCollectedAmount> WeeeCollectedAmounts { get; private set; }

        public virtual ICollection<AatfDeliveredAmount> AatfDeliveredAmounts { get; private set; }

        public virtual ICollection<AeDeliveredAmount> AeDeliveredAmounts { get; private set; }

        public virtual ICollection<EeeOutputAmount> EeeOutputAmounts { get; private set; }

        public DataReturnVersion(DataReturn dataReturn)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);

            DataReturn = dataReturn;

            WeeeCollectedAmounts = new List<WeeeCollectedAmount>();
            AatfDeliveredAmounts = new List<AatfDeliveredAmount>();
            AeDeliveredAmounts = new List<AeDeliveredAmount>();
            EeeOutputAmounts = new List<EeeOutputAmount>();
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected DataReturnVersion()
        {
        }

        public void AddWeeeCollectedAmount(WeeeCollectedAmount weeeCollectedAmount)
        {
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

        public void AddEeeOutputAmount(EeeOutputAmount eeeOutputAmount)
        {
            Guard.ArgumentNotNull(() => eeeOutputAmount, eeeOutputAmount);

            if (eeeOutputAmount.RegisteredProducer.Scheme.Id != DataReturn.Scheme.Id)
            {
                string errorMesage = "The specified producer was registered in a different scheme to this data return.";
                throw new InvalidOperationException(errorMesage);
            }

            EeeOutputAmounts.Add(eeeOutputAmount);
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

        public void Submit(string userId)
        {
            if (IsSubmitted)
            {
                string errorMessage = "This data return version has already been submitted.";
                throw new InvalidOperationException(errorMessage);
            }
            if (DataReturn != null)
            {
                IsSubmitted = true;
                SubmittedDate = SystemTime.UtcNow;
                SubmittingUserId = userId;
                DataReturn.SetCurrentVersion(this);
            }
            else
            {
                string errorMessage = "This data return version has no corresponding data return.";
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}