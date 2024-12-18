namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using Domain.Organisation;
    using EA.Weee.Core.Organisations;
    using Newtonsoft.Json;
    using Weee.Tests.Core;

    public class OrganisationTransactionDbSetup : DbTestDataBuilder<OrganisationTransaction, OrganisationTransactionDbSetup>
    {
        protected override OrganisationTransaction Instantiate()
        {
            var user = DbContext.GetCurrentUser();
            instance = new OrganisationTransaction(user);

            return instance;
        }

        public OrganisationTransactionDbSetup WithModel(OrganisationTransactionData organisationTransactionData)
        {
            ObjectInstantiator<OrganisationTransaction>.SetProperty(o => o.OrganisationJson, JsonConvert.SerializeObject(organisationTransactionData), instance);

            return this;
        }
    }
}
