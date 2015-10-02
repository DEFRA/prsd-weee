namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using DataAccess;
    using Domain.Producer;

    public class CurrentCompanyProducers : Query<List<Producer>>, ICurrentCompanyProducers
    {
        public CurrentCompanyProducers(WeeeContext context)
        {
            query = () => context
                .Producers
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Where(p => p.IsCurrentForComplianceYear &&
                            p.ProducerBusiness != null &&
                            p.ProducerBusiness.CompanyDetails != null)
                .AsNoTracking()
                .ToList();
        }
    }
}
