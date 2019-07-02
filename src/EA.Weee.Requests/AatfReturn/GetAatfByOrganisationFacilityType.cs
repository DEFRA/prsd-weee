namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfByOrganisationFacilityType : IRequest<List<AatfData>>
    {
        public Guid OrganisationId { get; set; }

        public FacilityType FacilityType { get; set; }

        public GetAatfByOrganisationFacilityType(Guid organisationId, FacilityType facilityType)
        {
            OrganisationId = organisationId;
            FacilityType = facilityType;
        }
    }
}
