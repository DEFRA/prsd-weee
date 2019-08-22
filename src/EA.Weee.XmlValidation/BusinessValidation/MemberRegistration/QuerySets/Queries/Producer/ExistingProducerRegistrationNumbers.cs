namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using DataAccess;
    using System.Collections.Generic;
    using System.Linq;

    public class ExistingProducerRegistrationNumbers : Query<List<string>>, IExistingProducerRegistrationNumbers
    {
        public ExistingProducerRegistrationNumbers(WeeeContext context)
        {
            query = () => context
                .RegisteredProducers
                .Where(rp => rp.CurrentSubmission != null)
                .Select(p => p.ProducerRegistrationNumber)
                .Distinct()
                .ToList();
        }
    }
}
