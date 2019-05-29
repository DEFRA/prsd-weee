namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System.Threading.Tasks;
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

    internal class EditAatfDetailsRequestHandler : IRequestHandler<EditAatfDetails, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditAatfDetailsRequestHandler(
            IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IGenericDataAccess genericDataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.addressMapper = addressMapper;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
        }

        public async Task<bool> HandleAsync(EditAatfDetails message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var siteAddress = addressMapper.Map(message.Data.SiteAddress);

            var existingAatf = await genericDataAccess.GetById<Aatf>(message.Data.Id);
            var updatedAatf = new Aatf(
                message.Data.Name,
                message.Data.CompetentAuthority.Id,
                message.Data.ApprovalNumber,
                Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Data.AatfStatus.Value),
                existingAatf.Operator,
                siteAddress,
                Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Data.Size.Value),
                message.Data.ApprovalDate.GetValueOrDefault(),
                existingAatf.Contact,
                existingAatf.FacilityType);

            Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.Data.SiteAddress.CountryId);

            await aatfDataAccess.UpdateDetails(existingAatf, updatedAatf, country);

            return true;
        }
    }
}
