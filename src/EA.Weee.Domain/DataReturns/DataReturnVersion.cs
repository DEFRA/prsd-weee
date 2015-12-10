namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class DataReturnVersion : Entity
    {        
        public virtual DataReturn DataReturn { get; private set; }

        public virtual DateTime? SubmittedDate { get; private set; }

        public string SubmittingUserId { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual ICollection<EeeOutputAmount> EeeOutputAmounts { get; private set; }

        public DataReturnVersion(DataReturn dataReturn)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);
            this.DataReturn = dataReturn;

            EeeOutputAmounts = new List<EeeOutputAmount>();
        }

        protected DataReturnVersion()
        {
        }

        public void AddEeeOutputAmount(EeeOutputAmount eeeOutputAmount)
        {
            Guard.ArgumentNotNull(() => eeeOutputAmount, eeeOutputAmount);

            EeeOutputAmounts.Add(eeeOutputAmount);
        }

        public void Submit(string userId)
        {
            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            IsSubmitted = true;
            SubmittedDate = SystemTime.UtcNow;
            SubmittingUserId = userId;
            DataReturn.SetCurrentDataReturnVersion(this);     
        }       
    }
}