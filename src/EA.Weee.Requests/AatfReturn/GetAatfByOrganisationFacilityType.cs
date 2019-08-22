namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

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
