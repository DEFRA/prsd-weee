namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using AatfSize = Domain.AatfReturn.AatfSize;
    using AatfStatus = Domain.AatfReturn.AatfStatus;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class AatfSimpleMap : IMap<AatfSimpleMapObject, AatfData>
    {
        private readonly IMap<AatfAddress, AatfAddressData> aatfAddressMap;
        private readonly IMap<AatfContact, AatfContactData> contactMap;
        private readonly IMap<Organisation, OrganisationData> organisationMap;
        private readonly IMap<AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;

        public AatfSimpleMap(IMap<AatfAddress, AatfAddressData> aatfAddressMap,
            IMap<AatfContact, AatfContactData> contactMap,
            IMap<Organisation, OrganisationData> organisationMap, 
            IMap<AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap)
        {
            this.aatfAddressMap = aatfAddressMap;
            this.contactMap = contactMap;
            this.organisationMap = organisationMap;
            this.aatfStatusMap = aatfStatusMap;
        }

        public AatfData Map(AatfSimpleMapObject source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var aatfStatus = aatfStatusMap.Map(source.Aatf.AatfStatus);

            var address = aatfAddressMap.Map(source.Aatf.SiteAddress);

            var contact = contactMap.Map(source.Aatf.Contact);

            var organisation = organisationMap.Map(source.Aatf.Organisation);

            var aatf = new AatfData()
            {
                Id = source.Aatf.Id,
                Name = source.Aatf.Name,
                ApprovalNumber = source.Aatf.ApprovalNumber,
                ComplianceYear = source.Aatf.ComplianceYear,
                SiteAddress = address,
                ApprovalDate = source.Aatf.ApprovalDate.GetValueOrDefault(),
                Contact = contact,
                Organisation = organisation,
                FacilityType = source.Aatf.FacilityType.ToCoreEnumeration<FacilityType>(),
                AatfId = source.Aatf.AatfId,
                AatfStatusDisplay = aatfStatus.ToDisplayString(),
                AatfStatus = aatfStatusMap.Map(source.Aatf.AatfStatus),
                HasEvidenceNotes = source.Aatf.Notes?.Any() ?? false
            };

            return aatf;
        }
    }
}
