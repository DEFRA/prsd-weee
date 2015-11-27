namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain.Producer;

    public class MigratedProducerQuerySet : IMigratedProducerQuerySet
    {
        private readonly WeeeContext context;
        
        private bool dataFetched = false;
        private List<string> existingMigratedProducerRegistrationNumbers;
        private List<MigratedProducer> migratedProducers; 

        public MigratedProducerQuerySet(WeeeContext context)
        {
            this.context = context;
        }

        private void EnsureDataFetched()
        {
            if (!dataFetched)
            {
                FetchData();
                dataFetched = true;
            }
        }

        private void FetchData()
        {
            existingMigratedProducerRegistrationNumbers = context
                .MigratedProducers
                .Select(mp => mp.ProducerRegistrationNumber)
                .Distinct()
                .ToList();

            migratedProducers = context
                .MigratedProducers
                .AsNoTracking()
                .ToList();
        }

        public List<string> GetAllRegistrationNumbers()
        {
            EnsureDataFetched();

            return existingMigratedProducerRegistrationNumbers;
        }

        public MigratedProducer GetMigratedProducer(string registrationNo)
        {
            EnsureDataFetched();

            return migratedProducers
                .SingleOrDefault(p => p.ProducerRegistrationNumber == registrationNo);
        }

        public List<MigratedProducer> GetAllMigratedProducers()
        {
            EnsureDataFetched();

            return migratedProducers;
        }
    }
}
