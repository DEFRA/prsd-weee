namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Collections.Generic;
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

        public Task<DataReturnVersion> GetLatestDataReturnVersionOrDefault()
        {
            return context.DataReturnVersions
                .Where(rv => rv.DataReturn.Scheme.Id == scheme.Id)
                .Where(rv => rv.DataReturn.Quarter.Year == quarter.Year)
                .Where(rv => rv.DataReturn.Quarter.Q == quarter.Q)
                .OrderByDescending(rv => rv.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<AatfDeliveryLocation> GetOrAddAatfDeliveryLocation(string approvalNumber, string facilityName)
        {
            var aatfDeliveryLocation =
                // Read from the local collection to retrieve items that have been added but not yet saved to the database.
                context.AatfDeliveryLocations.Local
                .Where(aatf => aatf.ApprovalNumber == approvalNumber)
                .Where(aatf => aatf.FacilityName == facilityName)
                .SingleOrDefault()
                ??
                await context.AatfDeliveryLocations
                .Where(aatf => aatf.ApprovalNumber == approvalNumber)
                .Where(aatf => aatf.FacilityName == facilityName)
                .SingleOrDefaultAsync();

            if (aatfDeliveryLocation == null)
            {
                aatfDeliveryLocation = new AatfDeliveryLocation(approvalNumber, facilityName);
                context.AatfDeliveryLocations.Add(aatfDeliveryLocation);
            }

            return aatfDeliveryLocation;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
        public async Task<AeDeliveryLocation> GetOrAddAeDeliveryLocation(string approvalNumber, string operatorName)
        {
            var aeDeliveryLocation =
                // Read from the local collection to retrieve items that have been added but not yet saved to the database.
                context.AeDeliveryLocations.Local
                .Where(ae => ae.ApprovalNumber == approvalNumber)
                .Where(ae => ae.OperatorName == operatorName)
                .SingleOrDefault()
                ??
                await context.AeDeliveryLocations
                .Where(ae => ae.ApprovalNumber == approvalNumber)
                .Where(ae => ae.OperatorName == operatorName)
                .SingleOrDefaultAsync();

            if (aeDeliveryLocation == null)
            {
                aeDeliveryLocation = new AeDeliveryLocation(approvalNumber, operatorName);
                context.AeDeliveryLocations.Add(aeDeliveryLocation);
            }

            return aeDeliveryLocation;
        }
    }
}
