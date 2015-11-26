namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using DataAccess.Extensions;

    public class ExistingProducerNames : Query<List<string>>, IExistingProducerNames
    {
        public ExistingProducerNames(WeeeContext context)
        {
            query = () => context
                .ProducerSubmissions
                .ProducerNames()
                .ToList();
        }
    }
}
