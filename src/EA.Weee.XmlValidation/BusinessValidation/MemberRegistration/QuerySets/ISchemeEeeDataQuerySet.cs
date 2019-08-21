namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain.DataReturns;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISchemeEeeDataQuerySet
    {
        Task<List<EeeOutputAmount>> GetLatestProducerEeeData(string registrationNumber);
    }
}
