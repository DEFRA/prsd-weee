namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using Domain.Producer;

    public interface ICurrentProducersByRegistrationNumber : IQuery<Dictionary<string, List<ProducerSubmission>>>
    {
    }
}
