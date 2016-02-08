namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Producer;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilderDataAccess : IDataReturnVersionBuilderDataAccess
    {
        private readonly WeeeContext context;
        private readonly Scheme scheme;
        private readonly Quarter quarter;
        private ICollection<RegisteredProducer> schemeYearProducers;
        private Dictionary<string, AatfDeliveryLocation> cachedAatfDeliveryLocations;
        private Dictionary<string, AeDeliveryLocation> cachedAeDeliveryLocations;

        public ReadOnlyDictionary<string, AatfDeliveryLocation> CachedAatfDeliveryLocations
        {
            get
            {
                return new ReadOnlyDictionary<string, AatfDeliveryLocation>(cachedAatfDeliveryLocations);
            }
        }

        public ReadOnlyDictionary<string, AeDeliveryLocation> CachedAeDeliveryLocations
        {
            get
            {
                return new ReadOnlyDictionary<string, AeDeliveryLocation>(cachedAeDeliveryLocations);
            }
        }

        public DataReturnVersionBuilderDataAccess(Scheme scheme, Quarter quarter, WeeeContext context)
        {
            this.context = context;
            this.scheme = scheme;
            this.quarter = quarter;
        }

        public Task<DataReturn> FetchDataReturnOrDefault()
        {
            return context.DataReturns
                .Where(dr => dr.Scheme.Id == scheme.Id)
                .Where(dr => dr.Quarter.Year == quarter.Year)
                .Where(dr => dr.Quarter.Q == quarter.Q)
                .SingleOrDefaultAsync();
        }

        public async Task<RegisteredProducer> GetRegisteredProducer(string producerRegistrationNumber)
        {
            ICollection<RegisteredProducer> producers = await GetSchemeYearProducers();
            return producers.SingleOrDefault(p => p.ProducerRegistrationNumber == producerRegistrationNumber);
        }

        private async Task<ICollection<RegisteredProducer>> GetSchemeYearProducers()
        {
            if (schemeYearProducers == null)
            {
                schemeYearProducers = await context.RegisteredProducers
                .Where(p => p.Scheme.Id == scheme.Id)
                .Where(p => p.ComplianceYear == quarter.Year)
                .Where(p => p.CurrentSubmission != null)
                .ToListAsync();
            }
            return schemeYearProducers;
        }

        public async Task<AatfDeliveryLocation> GetOrAddAatfDeliveryLocation(string approvalNumber, string facilityName)
        {
            // Replace empty strings with null
            facilityName = string.IsNullOrEmpty(facilityName) ? null : facilityName;

            if (cachedAatfDeliveryLocations == null)
            {
                cachedAatfDeliveryLocations =
                    await context.AatfDeliveryLocations
                    .ToDictionaryAsync(aatf => string.Format("{0}{1}", aatf.ApprovalNumber, aatf.FacilityName), StringComparer.OrdinalIgnoreCase);
            }

            var key = string.Format("{0}{1}", approvalNumber, facilityName);
            AatfDeliveryLocation aatfDeliveryLocation;
            if (!cachedAatfDeliveryLocations.TryGetValue(key, out aatfDeliveryLocation))
            {
                aatfDeliveryLocation = new AatfDeliveryLocation(approvalNumber, facilityName);

                cachedAatfDeliveryLocations.Add(key, aatfDeliveryLocation);
                context.AatfDeliveryLocations.Add(aatfDeliveryLocation);
            }

            return aatfDeliveryLocation;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
        public async Task<AeDeliveryLocation> GetOrAddAeDeliveryLocation(string approvalNumber, string operatorName)
        {
            // Replace empty strings with null
            operatorName = string.IsNullOrEmpty(operatorName) ? null : operatorName;

            if (cachedAeDeliveryLocations == null)
            {
                cachedAeDeliveryLocations =
                    await context.AeDeliveryLocations
                    .ToDictionaryAsync(ae => string.Format("{0}{1}", ae.ApprovalNumber, ae.OperatorName), StringComparer.OrdinalIgnoreCase);
            }

            var key = string.Format("{0}{1}", approvalNumber, operatorName);
            AeDeliveryLocation aeDeliveryLocation;
            if (!cachedAeDeliveryLocations.TryGetValue(key, out aeDeliveryLocation))
            {
                aeDeliveryLocation = new AeDeliveryLocation(approvalNumber, operatorName);

                cachedAeDeliveryLocations.Add(key, aeDeliveryLocation);
                context.AeDeliveryLocations.Add(aeDeliveryLocation);
            }

            return aeDeliveryLocation;
        }
    }
}
