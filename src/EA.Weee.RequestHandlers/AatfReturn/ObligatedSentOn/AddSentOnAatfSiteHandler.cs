namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class AddSentOnAatfSiteHandler : IRequestHandler<AddSentOnAatfSite, Guid>
    {
        private readonly WeeeContext context;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public AddSentOnAatfSiteHandler(WeeeContext context, IWeeeAuthorization authorization,
        IWeeeSentOnDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess, IReturnDataAccess returnDataAccess, IOrganisationDetailsDataAccess orgDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.organisationDetailsDataAccess = orgDataAccess;
        }

        public async Task<Guid> HandleAsync(AddSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOn = new WeeeSentOn();

            Country siteCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.SiteAddressData.CountryId);

            var @return = await returnDataAccess.GetById(message.ReturnId);

            var aatf = await genericDataAccess.GetById<Aatf>(message.AatfId);

            var siteAddress = new AatfAddress(
                message.SiteAddressData.Name,
                message.SiteAddressData.Address1,
                message.SiteAddressData.Address2,
                message.SiteAddressData.TownOrCity,
                message.SiteAddressData.CountyOrRegion,
                message.SiteAddressData.Postcode,
                siteCountry);

            weeeSentOn = new WeeeSentOn(siteAddress, aatf, @return);

            await sentOnDataAccess.Submit(weeeSentOn);

            return weeeSentOn.Id;
        }
    }
}