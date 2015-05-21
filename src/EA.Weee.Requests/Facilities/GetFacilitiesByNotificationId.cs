namespace EA.Weee.Requests.Facilities
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class GetFacilitiesByNotificationId : IRequest<IList<FacilityData>>
    {
        public Guid NotificationId { get; set; }

        public GetFacilitiesByNotificationId(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}