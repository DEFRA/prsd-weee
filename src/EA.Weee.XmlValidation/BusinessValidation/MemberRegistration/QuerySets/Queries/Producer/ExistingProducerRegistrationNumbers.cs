namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using DataAccess;
    using System.Collections.Generic;
    using System.Linq;

    public class ExistingProducerRegistrationNumbers : Query<List<string>>, IExistingProducerRegistrationNumbers
    {
        public ExistingProducerRegistrationNumbers(WeeeContext context)
        {
            query = () =>
            {
                var registeredProducerNumbers = context.RegisteredProducers
                    .Where(rp => rp.CurrentSubmission != null)
                    .Select(p => p.ProducerRegistrationNumber);

                var directProducerNumbers = context.DirectProducerSubmissions
                    .Select(a => a.RegisteredProducer.ProducerRegistrationNumber);

                return registeredProducerNumbers
                    .Union(directProducerNumbers)
                    .Distinct()
                    .ToList();
            };
        }
    }
}
