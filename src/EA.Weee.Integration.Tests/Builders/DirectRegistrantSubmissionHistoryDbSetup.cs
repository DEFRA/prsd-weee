namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;
    using System;

    public class DirectRegistrantSubmissionHistoryDbSetup : DbTestDataBuilder<DirectProducerSubmissionHistory, DirectRegistrantSubmissionHistoryDbSetup>
    {
        protected override DirectProducerSubmissionHistory Instantiate()
        {
            instance = new DirectProducerSubmissionHistory
            {
                CompanyName = Faker.Company.Name(),
                TradingName = Faker.Company.Name(),
                CompanyRegistrationNumber = Faker.RandomNumber.Next(0, 10).ToString()
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

        public DirectRegistrantSubmissionHistoryDbSetup WithBusinessAddress(Guid addressId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BusinessAddressId, addressId, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithContactAddress(Guid addressId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.ContactAddressId, addressId, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithBrandName(string brandName)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BrandName, new BrandName(brandName), instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithAuthorisedRep(AuthorisedRepresentative authorisedRep)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.AuthorisedRepresentative, authorisedRep, instance);

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

        public DirectRegistrantSubmissionHistoryDbSetup WithAppropriateSignatory(Contact appropriateSignatory)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.AppropriateSignatory, appropriateSignatory, instance);

            return this;
        }
    }
}
