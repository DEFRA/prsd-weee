namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
    using EA.Prsd.Core;
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates a <see cref="ProducerList"/> based on a collection of settings specifying the
    /// schema version, the compliance year, the number of new/existing producers etc.
    /// </summary>
    public class ProducerListFactory : IProducerListFactory
    {
        private struct SchemeInfo
        {
            public string TradingName { get; set; }
            public string ApprovalNumber { get; set; }
        }

        private WeeeContext context;

        public ProducerListFactory(WeeeContext context)
        {
            Guard.ArgumentNotNull(() => context, context);

            this.context = context;
        }

        public async Task<ProducerList> CreateAsync(ProducerListSettings listSettings)
        {
            Guard.ArgumentNotNull(() => listSettings, listSettings);

            ProducerList producerList = new ProducerList();

            SchemeInfo schemeInfo = await FetchSchemeInfo(listSettings.OrganisationID);

            producerList.SchemaVersion = listSettings.SchemaVersion;
            producerList.ApprovalNumber = schemeInfo.ApprovalNumber;
            producerList.ComplianceYear = listSettings.ComplianceYear;
            producerList.TradingName = schemeInfo.TradingName;
            producerList.SchemeBusiness = SchemeBusiness.Create(listSettings);

            for (int index = 0; index < listSettings.NumberOfNewProducers; ++index)
            {
                ProducerSettings producerSettings = new ProducerSettings();

                producerSettings.SchemaVersion = listSettings.SchemaVersion;
                producerSettings.IsNew = true;

                Producer producer = Producer.Create(producerSettings);

                producerList.Producers.Add(producer);
            }

            int numberOfExistingProducersToInclude = listSettings.NumberOfExistingProducers;

            // TODO: Ensure only database records representing "current" producers are returned.
            // Checking "IsSubmitted" here isn't sufficient as each producer may have several updates.
            List<string> registrationNumbers = await context
                .Producers
                .Where(p => p.MemberUpload.IsSubmitted)
                .Where(p => p.Scheme.OrganisationId == listSettings.OrganisationID)
                .Select(p => p.RegistrationNumber)
                .Take(numberOfExistingProducersToInclude)
                .ToListAsync();

            int numberOfExistingProducers = registrationNumbers.Count;

            if (numberOfExistingProducersToInclude > numberOfExistingProducers)
            {
                numberOfExistingProducersToInclude = numberOfExistingProducers;
            }

            for (int index = 0; index < numberOfExistingProducersToInclude; ++index)
            {
                ProducerSettings producerSettings = new ProducerSettings();

                producerSettings.SchemaVersion = listSettings.SchemaVersion;
                producerSettings.IsNew = false;
                producerSettings.RegistrationNumber = registrationNumbers[index];

                Producer producer = Producer.Create(producerSettings);

                producerList.Producers.Add(producer);
            }

            return producerList;
        }

        private async Task<SchemeInfo> FetchSchemeInfo(Guid organisationId)
        {
            var schemeInfos = await context
                .Schemes
                .Where(s => s.OrganisationId == organisationId)
                .Select(s => new { s.Organisation.TradingName, s.ApprovalNumber })
                .ToListAsync();

            if (schemeInfos.Count == 0)
            {
                string message = string.Format(
                    "No scheme was found in the database with an organisation ID of \"{0}\".",
                    organisationId);

                throw new Exception(message);
            }

            if (schemeInfos.Count > 1)
            {
                string message = string.Format(
                    "More than one scheme was found in the database with an organisation ID of \"{0}\".",
                    organisationId);

                throw new Exception(message);
            }

            var schemeInfo = schemeInfos.Single();

            return new SchemeInfo()
            {
                TradingName = schemeInfo.TradingName,
                ApprovalNumber = schemeInfo.ApprovalNumber
            };
        }
    }
}
