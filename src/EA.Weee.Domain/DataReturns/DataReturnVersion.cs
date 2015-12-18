namespace EA.Weee.Domain.DataReturns
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

        public virtual WeeeCollectedReturnVersion WeeeCollectedReturnVersion { get; private set; }

        public virtual WeeeDeliveredReturnVersion WeeeDeliveredReturnVersion { get; private set; }

        public virtual EeeOutputReturnVersion EeeOutputReturnVersion { get; private set; }

        public DataReturnVersion(DataReturn dataReturn)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);

            DataReturn = dataReturn;

            WeeeCollectedReturnVersion = new WeeeCollectedReturnVersion(this);
            WeeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion(this);
            EeeOutputReturnVersion = new EeeOutputReturnVersion(this);
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected DataReturnVersion()
        {
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