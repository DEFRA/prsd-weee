namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using DataAccess;
    using DataAccess.Extensions;
    using System.Collections.Generic;
    using System.Linq;

    public class ExistingProducerNames : Query<List<string>>, IExistingProducerNames
    {
        public ExistingProducerNames(WeeeContext context)
        {
            query = () => context
                .Producers
                .ProducerNames()
                .ToList();
        }
    }
}
