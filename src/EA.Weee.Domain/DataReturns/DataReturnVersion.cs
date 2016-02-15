namespace EA.Weee.Domain.DataReturns
{
    using System;
    using EA.Prsd.Core.Domain;
    using Events;
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

        public bool IsSubmitted
        {
            get { return SubmittedDate.HasValue; }
        }

        public DateTime CreatedDate { get; private set; }

        public virtual WeeeCollectedReturnVersion WeeeCollectedReturnVersion { get; private set; }

        public virtual WeeeDeliveredReturnVersion WeeeDeliveredReturnVersion { get; private set; }

        public virtual EeeOutputReturnVersion EeeOutputReturnVersion { get; private set; }

        public DataReturnVersion(DataReturn dataReturn)
            : this(dataReturn, new WeeeCollectedReturnVersion(), new WeeeDeliveredReturnVersion(),
                   new EeeOutputReturnVersion())
        {
        }

        public DataReturnVersion(DataReturn dataReturn, WeeeCollectedReturnVersion weeeCollectedReturnVersion,
            WeeeDeliveredReturnVersion weeeDeliveredReturnVersion, EeeOutputReturnVersion eeeOutputReturnVersion)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);

            DataReturn = dataReturn;

            WeeeCollectedReturnVersion = weeeCollectedReturnVersion;
            WeeeDeliveredReturnVersion = weeeDeliveredReturnVersion;
            EeeOutputReturnVersion = eeeOutputReturnVersion;

            CreatedDate = SystemTime.UtcNow;
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

            SubmittedDate = SystemTime.UtcNow;
            SubmittingUserId = userId;
            DataReturn.SetCurrentVersion(this);

            RaiseEvent(new SchemeDataReturnSubmissionEvent(this));
        }
    }
}