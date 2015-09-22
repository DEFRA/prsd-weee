namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;

    public class MigratedProducerQuerySet : IMigratedProducerQuerySet
    {
        private readonly PersistentQueryResult<List<string>> existingMigratedProducerRegistrationNumbers;  

        public MigratedProducerQuerySet(WeeeContext context)
        {
            existingMigratedProducerRegistrationNumbers = new PersistentQueryResult<List<string>>(() => context.MigratedProducers.Select(mp => mp.ProducerRegistrationNumber).ToList());
        }

        public List<string> GetAllRegistrationNumbers()
        {
            return existingMigratedProducerRegistrationNumbers.Get();
        }
    }
}
