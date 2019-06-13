namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Security;
    using Shared;
    using PanArea = Domain.Lookup.PanArea;

    internal class EditAatfDetailsRequestHandler : IRequestHandler<EditAatfDetails, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly ICommonDataAccess commonDataAccess;

        public EditAatfDetailsRequestHandler(
            IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IGenericDataAccess genericDataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess, 
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.addressMapper = addressMapper;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<bool> HandleAsync(EditAatfDetails message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var updatedAddress = addressMapper.Map(message.Data.SiteAddress);

            var existingAatf = await genericDataAccess.GetById<Aatf>(message.Data.Id);

            var competentAuthority = await commonDataAccess.FetchCompetentAuthority(message.Data.CompetentAuthority.Id);

            var localArea = await commonDataAccess.FetchLookup<LocalArea>(new Guid("65753EC0-623F-4319-999E-1AB6741DE98B"));

            var panArea = await commonDataAccess.FetchLookup<PanArea>(new Guid("65753EC0-623F-4319-999E-1AB6741DE98B"));

            var updatedAatf = new Aatf(
                message.Data.Name,
                competentAuthority,
                message.Data.ApprovalNumber,
                Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Data.AatfStatus.Value),
                existingAatf.Organisation,
                updatedAddress,
                Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Data.Size.Value),
                message.Data.ApprovalDate.GetValueOrDefault(),
                existingAatf.Contact,
                existingAatf.FacilityType,
                existingAatf.ComplianceYear,
                localArea,
                panArea);

            var existingAddress = await genericDataAccess.GetById<AatfAddress>(existingAatf.SiteAddress.Id);

            var country = await organisationDetailsDataAccess.FetchCountryAsync(message.Data.SiteAddress.CountryId);

            await aatfDataAccess.UpdateAddress(existingAddress, updatedAddress, country);

            await aatfDataAccess.UpdateDetails(existingAatf, updatedAatf);

            return true;
        }
    }
}
