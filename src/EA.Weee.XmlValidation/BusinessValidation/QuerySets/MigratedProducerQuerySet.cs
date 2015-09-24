namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain.Producer;

    public class MigratedProducerQuerySet : IMigratedProducerQuerySet
    {
        private readonly PersistentQueryResult<List<string>> existingMigratedProducerRegistrationNumbers;
        private readonly PersistentQueryResult<List<MigratedProducer>> migratedProducers; 

        public MigratedProducerQuerySet(WeeeContext context)
        {
            existingMigratedProducerRegistrationNumbers = new PersistentQueryResult<List<string>>(() => context.MigratedProducers.Select(mp => mp.ProducerRegistrationNumber).Distinct().ToList());
            migratedProducers = new PersistentQueryResult<List<MigratedProducer>>(() => context.MigratedProducers.ToList());
        }

        public List<string> GetAllRegistrationNumbers()
        {
            return existingMigratedProducerRegistrationNumbers.Get();
        }

        public MigratedProducer GetMigratedProducer(string registrationNo)
        {
            return migratedProducers.Get().SingleOrDefault(p => p.ProducerRegistrationNumber == registrationNo);
        }
    }
}
