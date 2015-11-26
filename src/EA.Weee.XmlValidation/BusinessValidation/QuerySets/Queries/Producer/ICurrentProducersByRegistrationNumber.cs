namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using Domain.Producer;

    public interface ICurrentProducersByRegistrationNumber : IQuery<Dictionary<string, List<ProducerSubmission>>>
    {
    }
}
