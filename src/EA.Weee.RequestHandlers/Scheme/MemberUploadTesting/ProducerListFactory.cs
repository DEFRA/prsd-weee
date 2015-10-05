namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Core.Scheme.MemberUploadTesting;
    using DataAccess;
    using Prsd.Core;

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

            string approvalNumber = SanitizeApprovalNumber(schemeInfo.ApprovalNumber);
            string tradingName = SanitizeTradingName(schemeInfo.TradingName);

            producerList.SchemaVersion = listSettings.SchemaVersion;
            producerList.ApprovalNumber = approvalNumber;
            producerList.ComplianceYear = listSettings.ComplianceYear;
            producerList.TradingName = tradingName;
            producerList.SchemeBusiness = SchemeBusiness.Create(listSettings);

            for (int index = 0; index < listSettings.NumberOfNewProducers; ++index)
            {
                ProducerSettings producerSettings = new ProducerSettings();

                producerSettings.SchemaVersion = listSettings.SchemaVersion;
                producerSettings.IsNew = true;
                producerSettings.IgnoreStringLengthConditions = listSettings.IgnoreStringLengthConditions;

                Producer producer = Producer.Create(producerSettings, listSettings.NoCompaniesForNewProducers);

                producerList.Producers.Add(producer);
            }

            int numberOfExistingProducersToInclude = listSettings.NumberOfExistingProducers;

            List<string> registrationNumbers = await context
                .Producers
                .Where(p => p.IsCurrentForComplianceYear)
                .Where(p => p.MemberUpload.ComplianceYear == listSettings.ComplianceYear)
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
                producerSettings.IgnoreStringLengthConditions = listSettings.IgnoreStringLengthConditions;

                Producer producer = Producer.Create(producerSettings, listSettings.NoCompaniesForNewProducers);

                producerList.Producers.Add(producer);
            }

            return producerList;
        }

        private static string SanitizeTradingName(string tradingName)
        {
            if (tradingName == null)
            {
                return string.Empty;
            }

            if (tradingName.Length > 2048)
            {
                return tradingName.Substring(0, 2048);
            }
            else
            {
                return tradingName;
            }
        }

        private static string SanitizeApprovalNumber(string approvalNumber)
        {
            Regex regex = new Regex("(WEE/)[A-Z]{2}[0-9]{4}[A-Z]{2}(/SCH)");

            if (!regex.IsMatch(approvalNumber))
            {
                return "WEE/ZZ9999ZZ/SCH";
            }
            else
            {
                return approvalNumber;
            }
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
