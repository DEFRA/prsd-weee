namespace EA.Weee.Domain.Events
{
    using DataReturns;
    using Prsd.Core.Domain;

    public class SchemeDataReturnSubmissionEvent : IEvent
    {
        public DataReturnVersion DataReturnVersion { get; private set; }

        public SchemeDataReturnSubmissionEvent(DataReturnVersion dataReturnVersion)
        {
            DataReturnVersion = dataReturnVersion;
        }
    }
}
