namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using DataAccess;
    using Domain.Producer;

    public class CurrentCompanyProducers : Query<List<ProducerSubmission>>, ICurrentCompanyProducers
    {
        public CurrentCompanyProducers(WeeeContext context)
        {
            query = () => context
                .RegisteredProducers
                .Where(rp => rp.CurrentSubmission != null)
                .Select(rp => rp.CurrentSubmission)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Where(p => p.ProducerBusiness != null &&
                            p.ProducerBusiness.CompanyDetails != null)
                .AsNoTracking()
                .ToList();
        }
    }
}
