namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class AddSentOnAatfSiteHandler : IRequestHandler<AddSentOnAatfSite, Guid>
    {
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public AddSentOnAatfSiteHandler(IWeeeAuthorization authorization,
        IWeeeSentOnDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess, IReturnDataAccess returnDataAccess, IOrganisationDetailsDataAccess orgDataAccess)
        {
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.organisationDetailsDataAccess = orgDataAccess;
        }

        public async Task<Guid> HandleAsync(AddSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var siteCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.SiteAddressData.CountryId);
            var operatorCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.OperatorAddressData.CountryId);

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

            var operatorAddress = new AatfAddress(
                message.OperatorAddressData.Name,
                message.OperatorAddressData.Address1,
                message.OperatorAddressData.Address2,
                message.OperatorAddressData.TownOrCity,
                message.OperatorAddressData.CountyOrRegion,
                message.OperatorAddressData.Postcode,
                operatorCountry);

            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            await sentOnDataAccess.Submit(weeeSentOn);

            return weeeSentOn.Id;
        }
    }
}