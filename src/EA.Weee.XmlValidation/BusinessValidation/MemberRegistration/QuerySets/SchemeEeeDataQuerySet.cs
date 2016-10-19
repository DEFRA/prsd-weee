namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Obligation;
    using EA.Weee.Domain.DataReturns;

    public class SchemeEeeDataQuerySet : ISchemeEeeDataQuerySet
    {
        private readonly string schemeApprovalNumber;
        private readonly int complianceYear;
        private readonly WeeeContext context;
        private Dictionary<string, List<EeeOutputAmount>> producerEeeOutputAmount;

        public SchemeEeeDataQuerySet(string schemeApprovalNumber, int complianceYear, WeeeContext context)
        {
            this.schemeApprovalNumber = schemeApprovalNumber;
            this.complianceYear = complianceYear;
            this.context = context;
        }

        public async Task<bool> HasProducerEeeDataForObligationType(string registrationNumber, ObligationType obligationType)
        {
            if (producerEeeOutputAmount == null)
            {
                producerEeeOutputAmount = await
                    context.DataReturnVersions
                    .Where(d => d.DataReturn.Scheme.ApprovalNumber == schemeApprovalNumber)
                    .Where(d => d.DataReturn.Quarter.Year == complianceYear)
                    .SelectMany(d => d.EeeOutputReturnVersion.EeeOutputAmounts)
                    .GroupBy(e => e.RegisteredProducer.ProducerRegistrationNumber)
                    .ToDictionaryAsync(k => k.Key, e => e.ToList());
            }

            bool result = false;
            List<EeeOutputAmount> producerAmounts;

            if (producerEeeOutputAmount.TryGetValue(registrationNumber, out producerAmounts))
            {
                result = producerAmounts.Any(p => p.ObligationType == obligationType);
            }

            return result;
        }
    }
}
