namespace EA.Weee.Domain.Events
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.AatfReturn;

    public class AatfContactDetailsUpdateEvent : IEvent
    {
        public Aatf Aatf { get; }

        public AatfContactDetailsUpdateEvent(Aatf aatf)
        {
            this.Aatf = aatf;
        }
    }
}
