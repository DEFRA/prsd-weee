﻿namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Domain.Lookup;
    using Organisations;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using Shared;
    using Weee.Security;
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

            var competentAuthority = await commonDataAccess.FetchCompetentAuthority(message.Data.CompetentAuthority.Abbreviation);

            LocalArea localArea = null;
            PanArea panArea = null;

            if (message.Data.LocalAreaData != null)
            {
                localArea = await commonDataAccess.FetchLookup<LocalArea>(message.Data.LocalAreaData.Id);
            }

            if (message.Data.PanAreaData != null)
            {
                panArea = await commonDataAccess.FetchLookup<PanArea>(message.Data.PanAreaData.Id);
            } 

            var updatedAatf = new Aatf(
                message.Data.Name,
                competentAuthority,
                message.Data.ApprovalNumber,
                Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Data.AatfStatusValue),
                existingAatf.Organisation,
                updatedAddress,
                Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Data.AatfSizeValue),
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
