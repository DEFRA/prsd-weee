namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.AatfReturn;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using System;

    public class AatfMap : IMap<Aatf, AatfData>
    {
        private readonly IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;
        private readonly IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap;
        private readonly IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap;
        private readonly IMap<AatfAddress, AatfAddressData> aatfAddressMap;
        private readonly IMap<Operator, OperatorData> operatorMap;
        private readonly IMap<AatfContact, AatfContactData> contactMap;
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public AatfMap(IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap,
            IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus> aatfStatusMap,
            IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize> aatfSizeMap,
            IMap<AatfAddress, AatfAddressData> aatfAddressMap,
            IMap<Operator, OperatorData> operatorMap,
            IMap<AatfContact, AatfContactData> contactMap,
            IMap<Organisation, OrganisationData> organisationMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
            this.aatfStatusMap = aatfStatusMap;
            this.aatfSizeMap = aatfSizeMap;
            this.aatfAddressMap = aatfAddressMap;
            this.operatorMap = operatorMap;
            this.contactMap = contactMap;
            this.organisationMap = organisationMap;
        }

        public AatfData Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            UKCompetentAuthorityData compentAuthority = competentAuthorityMap.Map(source.CompetentAuthority);

            OperatorData @operator = operatorMap.Map(source.Operator);

            Core.AatfReturn.AatfStatus aatfStatus = aatfStatusMap.Map(source.AatfStatus);

            Core.AatfReturn.AatfSize aatfSize = aatfSizeMap.Map(source.Size);

            AatfAddressData address = aatfAddressMap.Map(source.SiteAddress);

            AatfContactData contact = contactMap.Map(source.Contact);

            OrganisationData organisation = organisationMap.Map(source.Operator.Organisation);

            return new AatfData(source.Id, source.Name, source.ApprovalNumber, compentAuthority, aatfStatus, address, aatfSize, source.ApprovalDate.GetValueOrDefault(), source.ComplianceYear)
            {
                Contact = contact,
                Organisation = organisation,
                Operator = @operator
            };
        }
    }
}
