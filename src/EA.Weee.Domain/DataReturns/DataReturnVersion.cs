namespace EA.Weee.Domain.DataReturns
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class DataReturnVersion : Entity
    {        
        public virtual DataReturn DataReturn { get; private set; }

        public virtual DateTime? SubmittedDate { get; private set; }

        public virtual bool IsSubmitted { get; private set; }
        public DataReturnVersion(DataReturn dataReturn)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);
            this.DataReturn = dataReturn;            
        }

        protected DataReturnVersion()
        {
            IsSubmitted = false;
            SubmittedDate = null;
        }
        public void Submit()
        {
            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            IsSubmitted = true;
            SubmittedDate = SystemTime.UtcNow;
            DataReturn.SetCurrentDataReturnVersion(this);     
        }       
    }
}