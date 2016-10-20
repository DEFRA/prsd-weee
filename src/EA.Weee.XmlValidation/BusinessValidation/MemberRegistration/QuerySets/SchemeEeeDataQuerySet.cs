namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class SchemeEeeDataQuerySet : ISchemeEeeDataQuerySet
    {
        private readonly Guid organisationId;
        private readonly int complianceYear;
        private readonly WeeeContext context;
        private Dictionary<string, List<EeeOutputAmount>> producerEeeOutputAmount;

        public SchemeEeeDataQuerySet(Guid organisationId, string complianceYear, WeeeContext context)
        {
            this.organisationId = organisationId;
            this.complianceYear = int.Parse(complianceYear);
            this.context = context;
        }

        public async Task<List<EeeOutputAmount>> GetLatestProducerEeeData(string registrationNumber)
        {
            if (producerEeeOutputAmount == null)
            {
                producerEeeOutputAmount = await
                    context.DataReturns
                    .Where(d => d.Scheme.OrganisationId == organisationId)
                    .Where(d => d.Quarter.Year == complianceYear)
                    .Where(d => d.CurrentVersion != null)
                    .Where(d => d.CurrentVersion.EeeOutputReturnVersion != null)
                    .SelectMany(d => d.CurrentVersion.EeeOutputReturnVersion.EeeOutputAmounts)
                    .GroupBy(e => e.RegisteredProducer.ProducerRegistrationNumber)
                    .ToDictionaryAsync(k => k.Key, e => e.ToList());
            }

            List<EeeOutputAmount> producerAmounts;
            producerEeeOutputAmount.TryGetValue(registrationNumber, out producerAmounts);

            return producerAmounts;
        }
    }
}
