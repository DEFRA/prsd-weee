namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System;

    public class DirectRegistrantSubmissionHistoryDbSetup : DbTestDataBuilder<DirectProducerSubmissionHistory, DirectRegistrantSubmissionHistoryDbSetup>
    {
        protected override DirectProducerSubmissionHistory Instantiate()
        {
            instance = new DirectProducerSubmissionHistory
            {
                CompanyName = "new company name",
                TradingName = "new trading name",
                CompanyRegistrationNumber = "12345678"
            };

            return instance;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithDirectProducerSubmission(DirectProducerSubmission submission)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.DirectProducerSubmissionId, submission.Id, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithContact(Guid contactId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.ContactId, contactId, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithAddress(Guid addressId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BusinessAddressId, addressId, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithBrandName(string brandName)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BrandName, new BrandName(brandName), instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithAuthorisedRep(Guid authorisedRepId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.AuthorisedRepresentativeId, authorisedRepId, instance);

            return this;
        }
    }
}
