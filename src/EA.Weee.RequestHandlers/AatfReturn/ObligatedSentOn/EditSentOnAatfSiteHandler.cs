namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class EditSentOnAatfSiteHandler : IRequestHandler<EditSentOnAatfSite, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditSentOnAatfSiteHandler(IWeeeAuthorization authorization, IGenericDataAccess genericDataAccess, IOrganisationDetailsDataAccess orgDataAccess, IAatfSiteDataAccess offSiteDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.organisationDetailsDataAccess = orgDataAccess;
            this.offSiteDataAccess = offSiteDataAccess;
        }

        public async Task<Guid> HandleAsync(EditSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var siteCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.SiteAddressData.CountryId);

            var existingSiteAddress = await genericDataAccess.GetById<AatfAddress>(message.SiteAddressData.Id);

            var siteAddress = new SiteAddressData()
            {
                Name = message.SiteAddressData.Name,
                Address1 = message.SiteAddressData.Address1,
                Address2 = message.SiteAddressData.Address2,
                TownOrCity = message.SiteAddressData.TownOrCity,
                CountyOrRegion = message.SiteAddressData.CountyOrRegion,
                Postcode = message.SiteAddressData.Postcode,
                CountryName = message.SiteAddressData.CountryName,
                CountryId = message.SiteAddressData.CountryId
            };

            await offSiteDataAccess.Update(existingSiteAddress, siteAddress, siteCountry);

            var operatorCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.OperatorAddressData.CountryId);

            var existingOperatorAddress = await genericDataAccess.GetById<AatfAddress>(message.OperatorAddressData.Id);

            var operatorAddress = new OperatorAddressData()
            {
                Name = message.OperatorAddressData.Name,
                Address1 = message.OperatorAddressData.Address1,
                Address2 = message.OperatorAddressData.Address2,
                TownOrCity = message.OperatorAddressData.TownOrCity,
                CountyOrRegion = message.OperatorAddressData.CountyOrRegion,
                Postcode = message.OperatorAddressData.Postcode,
                CountryName = message.OperatorAddressData.CountryName,
                CountryId = message.OperatorAddressData.CountryId
            };

            await offSiteDataAccess.Update(existingOperatorAddress, operatorAddress, operatorCountry);

            return message.WeeeSentOnId;
        }
    }
}
