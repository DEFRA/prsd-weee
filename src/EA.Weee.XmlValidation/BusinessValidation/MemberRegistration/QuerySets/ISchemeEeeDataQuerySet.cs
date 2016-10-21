namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface ISchemeEeeDataQuerySet
    {
        Task<List<EeeOutputAmount>> GetLatestProducerEeeData(string registrationNumber);
    }
}
