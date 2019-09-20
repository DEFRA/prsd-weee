namespace EA.Weee.DataAccess.Extensions
{
    using Domain.Producer;
    using System.Linq;

    public static class ProducerSubmissionExtensions
    {
        public static IQueryable<string> ProducerNames(this IQueryable<ProducerSubmission> producers)
        {
            return producers.Select(p => p.ProducerBusiness != null
                ? p.ProducerBusiness.CompanyDetails != null
                    ? p.ProducerBusiness.CompanyDetails.Name
                    : p.ProducerBusiness.Partnership != null
                        ? p.ProducerBusiness.Partnership.Name
                        : p.TradingName
                : p.TradingName);
        }
    }
}
