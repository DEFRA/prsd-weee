namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using Domain.Producer;
    using System.Collections.Generic;

    public interface ICurrentProducersByRegistrationNumber : IQuery<Dictionary<string, List<Producer>>>
    {
    }
}
