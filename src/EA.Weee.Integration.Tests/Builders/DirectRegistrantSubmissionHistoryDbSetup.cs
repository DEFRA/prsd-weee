namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;
    using System;
    using EA.Weee.Domain.DataReturns;

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

        public DirectRegistrantSubmissionHistoryDbSetup WithSellingTechnique(SellingTechniqueType sellingTechnique)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.SellingTechniqueType, sellingTechnique.Value, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithEeeData(EeeOutputReturnVersion outputReturnVersion)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.EeeOutputReturnVersion, outputReturnVersion, instance);

            return this;
        }
    }
}
