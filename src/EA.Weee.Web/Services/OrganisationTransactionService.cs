namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    public class OrganisationTransactionService : IOrganisationTransactionService
    {
        private readonly Func<IWeeeClient> weeeClient;

        public OrganisationTransactionService(Func<IWeeeClient> weeeClient)
        {
            this.weeeClient = weeeClient;
        }

        public async Task CaptureData<T>(string accessToken, T model) where T : class
        {
            using (var client = weeeClient())
            {
                var transaction = await client.SendAsync(accessToken,  new GetUserOrganisationTransaction());

                if (model is OrganisationDetails details)
                {
                    transaction.OrganisationDetails = details;
                    //_currentSession.Screen1Metadata = metadata;
                }
            }
        }
    }
}