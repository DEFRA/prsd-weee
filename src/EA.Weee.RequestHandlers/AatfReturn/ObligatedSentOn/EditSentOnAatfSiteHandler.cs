namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditSentOnAatfSiteHandler : IRequestHandler<EditSentOnAatfSite, Guid>
    {
        private readonly WeeeContext context;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditSentOnAatfSiteHandler(WeeeContext context, IWeeeAuthorization authorization,
        ISentOnAatfSiteDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess, IReturnDataAccess returnDataAccess, IOrganisationDetailsDataAccess orgDataAccess, IAatfSiteDataAccess offSiteDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.organisationDetailsDataAccess = orgDataAccess;
            this.offSiteDataAccess = offSiteDataAccess;
        }

        public async Task<Guid> HandleAsync(EditSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();
            if (message.SiteAddressData != null)
            {
                Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.SiteAddressData.CountryId);

                var value = await genericDataAccess.GetById<AatfAddress>(message.SiteAddressId);

                var addressData = new SiteAddressData()
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

                await offSiteDataAccess.Update(value, addressData, country);
            }
            else
            {
                Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.OperatorAddressData.CountryId);

                var value = await genericDataAccess.GetById<AatfAddress>(message.OperatorAddressId);

                var addressData = new OperatorAddressData()
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

                await offSiteDataAccess.Update(value, addressData, country);
            }

            return message.WeeeSentOnId;
        }
    }
}
