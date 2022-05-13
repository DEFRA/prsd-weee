namespace EA.Weee.RequestHandlers.Admin
{
    using Domain.Lookup;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using Shared;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class AddAatfRequestHandler : IRequestHandler<AddAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;
        private readonly ICommonDataAccess commonDataAccess;

        public AddAatfRequestHandler(
            IWeeeAuthorization authorization,
            IGenericDataAccess dataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper,
            IMap<AatfContactData, AatfContact> contactMapper,
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.addressMapper = addressMapper;
            this.contactMapper = contactMapper;
            this.commonDataAccess = commonDataAccess;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(AddAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var siteAddress = addressMapper.Map(message.Aatf.SiteAddress);

            var organisation = await dataAccess.GetById<Organisation>(message.OrganisationId);

            var competentAuthority = await commonDataAccess.FetchCompetentAuthority(message.Aatf.CompetentAuthority.Abbreviation);

            var contact = contactMapper.Map(message.AatfContact);

            LocalArea localArea = null;

            PanArea panArea = null;

            if (message.Aatf.LocalAreaData != null)
            {
                localArea = await commonDataAccess.FetchLookup<LocalArea>(message.Aatf.LocalAreaData.Id);
            }

            if (message.Aatf.PanAreaData != null)
            {
                panArea = await commonDataAccess.FetchLookup<PanArea>(message.Aatf.PanAreaData.Id);
            }

            var aatf = new Domain.AatfReturn.Aatf(
            message.Aatf.Name,
            competentAuthority,
            message.Aatf.ApprovalNumber,
            Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Aatf.AatfStatus.Value),
            organisation,
            siteAddress,
            Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Aatf.Size.Value),
            message.Aatf.ApprovalDate.GetValueOrDefault(),
            contact,
            message.Aatf.FacilityType.ToDomainEnumeration<Domain.AatfReturn.FacilityType>(),
            message.Aatf.ComplianceYear,
            localArea,
            panArea, message.AatfId);

            await dataAccess.Add<Domain.AatfReturn.Aatf>(aatf);

            return true;
        }
    }
}
