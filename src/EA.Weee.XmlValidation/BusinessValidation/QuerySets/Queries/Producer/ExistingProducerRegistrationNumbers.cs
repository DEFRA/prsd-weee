namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;

    public class ExistingProducerRegistrationNumbers : Query<List<string>>, IExistingProducerRegistrationNumbers
    {
        public ExistingProducerRegistrationNumbers(WeeeContext context)
        {
            query = () => context
                .Producers
                .Select(p => p.RegistrationNumber)
                .Distinct()
                .ToList();
        }
    }
}
