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
    using System.Threading.Tasks;

    public class EditSentOnAatfSiteHandler : IRequestHandler<EditSentOnAatfSite, Guid>
    {
        private readonly WeeeContext context;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditSentOnAatfSiteHandler(WeeeContext context, IWeeeAuthorization authorization,
            ISentOnAatfSiteDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess, IReturnDataAccess returnDataAccess, IOrganisationDetailsDataAccess orgDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.organisationDetailsDataAccess = orgDataAccess;
        }

        public async Task<Guid> HandleAsync(EditSentOnAatfSite message)
        {
            var sentOn = await genericDataAccess.GetById<WeeeSentOn>(message.WeeeSentOnId);

            Country operatorCountry = await organisationDetailsDataAccess.FetchCountryAsync(message.OperatorAddressData.CountryId);

            var operatorAddress = new AatfAddress(
                message.OperatorAddressData.Name,
                message.OperatorAddressData.Address1,
                message.OperatorAddressData.Address2,
                message.OperatorAddressData.TownOrCity,
                message.OperatorAddressData.CountyOrRegion,
                message.OperatorAddressData.Postcode,
                operatorCountry);

            await sentOnDataAccess.UpdateWithOperatorAddress(sentOn, operatorAddress);

            return sentOn.Id;
        }
    }
}
