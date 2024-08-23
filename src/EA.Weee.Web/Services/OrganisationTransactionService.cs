﻿namespace EA.Weee.Web.Services
{
    using Azure.Core;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
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
                var transaction = await client.SendAsync(accessToken,  new GetUserOrganisationTransaction()) ?? new OrganisationTransactionData();

                switch (model)
                {
                    case OrganisationDetails details:
                        transaction.OrganisationDetails = details;
                        break;
                    case TonnageTypeViewModel tonnageTypeViewModel:
                        transaction.TonnageType = tonnageTypeViewModel.SelectedValue;
                        transaction.SearchTerm = tonnageTypeViewModel.SearchedText;
                        break;
                    case PreviousRegistrationViewModel previousRegistrationModel:
                        transaction.PreviousRegistration = previousRegistrationModel.SelectedValue;
                        break;
                }

                await client.SendAsync(accessToken, new AddUpdateOrganisationTransaction(transaction));
            }
        }

        public async Task<OrganisationTransactionData> GetOrganisationTransactionData(string accessToken)
        {
            using (var client = weeeClient())
            {
                var transaction = await client.SendAsync(accessToken, new GetUserOrganisationTransaction()) ?? new OrganisationTransactionData();

                return transaction;
            }
        }

        public async Task DeleteOrganisationTransactionData(string accessToken)
        {
            using (var client = weeeClient())
            {
                await client.SendAsync(accessToken, new DeleteUserOrganisationTransaction());
            }
        }
    }
}