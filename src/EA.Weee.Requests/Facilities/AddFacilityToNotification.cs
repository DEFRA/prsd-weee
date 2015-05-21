namespace EA.Weee.Requests.Facilities
{
    using System;
    using Prsd.Core.Mediator;

    public class AddFacilityToNotification : IRequest<Guid>
    {
        public FacilityData Facility { get; private set; }

        public AddFacilityToNotification(FacilityData facility)
        {
            this.Facility = facility;
        }
    }
}
