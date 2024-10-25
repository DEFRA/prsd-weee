namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DirectRegistrantSubmissionHistoryDbSetup : DbTestDataBuilder<DirectProducerSubmissionHistory, DirectRegistrantSubmissionHistoryDbSetup>
    {
        protected override DirectProducerSubmissionHistory Instantiate()
        {
            instance = new DirectProducerSubmissionHistory
            {
                CompanyName = Faker.Company.Name(),
                TradingName = Faker.Company.Name(),
                CompanyRegistrationNumber = Faker.RandomNumber.Next(1000000, 100000000000).ToString()
            };

            return instance;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithDirectProducerSubmission(DirectProducerSubmission submission)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.DirectProducerSubmissionId, submission.Id, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithContact(Contact contact)
        {
            instance.Contact = contact;

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithBusinessAddress(Guid addressId)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BusinessAddressId, addressId, instance);

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithContactAddress(Address address)
        {
            if (address.Country != null)
            {
                var country = DbContext.Countries.First(c => c.Id == address.Country.Id);
                address.Country = country;
                DbContext.Entry(country).State = EntityState.Unchanged;
            }

            // Attach the address if it's not already tracked
            //var addressEntry = DbContext.Entry(address);
            //if (addressEntry.State == EntityState.Detached)
            //{
                //DbContext.Set<Address>().Attach(address);
            //}

            instance.ContactAddress = address;

            return this;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithBrandName(string brandName)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.BrandName, new BrandName(brandName), instance);

            return this;
        }

        private void EnsureEntityIsTracked<T>(T entity) where T : class
        {
            var set = DbContext.Set<T>();
            if (!set.Local.Contains(entity))
            {
                var entry = DbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    set.Attach(entity);
                }
            }
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithAuthorisedRep(AuthorisedRepresentative authorisedRep)
        {
            if (!DbContext.Set<AuthorisedRepresentative>().Local.Contains(authorisedRep))
            {
                DbContext.Set<AuthorisedRepresentative>().Attach(authorisedRep);
            }

            // Then set the relationship
            instance.AuthorisedRepresentative = authorisedRep;

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
