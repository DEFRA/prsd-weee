namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using DataAccess;
    using Domain.Producer;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class CurrentProducersByRegistrationNumber : Query<Dictionary<string, List<Producer>>>, ICurrentProducersByRegistrationNumber
    {
        public CurrentProducersByRegistrationNumber(WeeeContext context)
        {
            query = () => context
                .Producers
                .Include(p => p.MemberUpload)
                .Include(p => p.Scheme)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Include(p => p.ProducerBusiness.Partnership)
                .Where(p => p.IsCurrentForComplianceYear)
                .AsNoTracking()
                .GroupBy(p => p.RegistrationNumber)
                .ToDictionary(g => g.Key, p => p.ToList());
        }
    }
}
