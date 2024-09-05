namespace EA.Weee.Web.Services
{
    using Azure.Core;
    using EA.Prsd.Core.Extensions;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.ViewModels.OrganisationRegistration;
    using EA.Weee.Web.ViewModels.OrganisationRegistration.Type;
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Core.Organisations.Base;

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
                    case OrganisationViewModel organisationViewModel:
                        transaction.OrganisationViewModel = organisationViewModel;
                            break;
                    case OrganisationTypeViewModel externalOrganisationTypeViewModel:
                        transaction.OrganisationType = externalOrganisationTypeViewModel.SelectedValue.GetValueFromDisplayName<ExternalOrganisationType>();
                        break;
                    case TonnageTypeViewModel tonnageTypeViewModel:
                        transaction.TonnageType = tonnageTypeViewModel.SelectedValue.GetValueFromDisplayName<TonnageType>();
                        transaction.SearchTerm = tonnageTypeViewModel.SearchedText;
                        break;
                    case PreviousRegistrationViewModel previousRegistrationModel:
                        transaction.PreviousRegistration = previousRegistrationModel.SelectedValue.GetValueFromDisplayName<YesNoType>();
                        break;
                    case RepresentingCompanyDetailsViewModel representingCompanyDetailsViewModel:
                        transaction.RepresentingCompanyDetailsViewModel = representingCompanyDetailsViewModel;
                        break;
                    case AuthorisedRepresentativeViewModel authorisedRepresentativeViewModel:
                        transaction.AuthorisedRepresentative = authorisedRepresentativeViewModel.SelectedValue.GetValueFromDisplayName<YesNoType>();
                        break;
                    case ContactDetailsViewModel contactDetailsViewModel:
                        transaction.ContactDetailsViewModel = contactDetailsViewModel;
                        break;
                    case PartnerViewModel partnerViewModel:
                        transaction.PartnerModels = partnerViewModel.AllParterModels;
                        break;
                }

                await client.SendAsync(accessToken, new AddUpdateOrganisationTransaction(transaction));
            }
        }

        public async Task<Guid> CompleteTransaction(string accessToken)
        {
            using (var client = weeeClient())
            {
                return await client.SendAsync(accessToken, new CompleteOrganisationTransaction());
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