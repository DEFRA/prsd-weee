﻿namespace EA.Weee.DataAccess.StoredProcedure
{
    using CuttingEdge.Conditions;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class EvidenceStoredProcedures : IEvidenceStoredProcedures
    {
        private readonly WeeeContext context;

        public EvidenceStoredProcedures(WeeeContext context)
        {
            this.context = context;

            var cmdTimeout = -1;
            var timeoutSettings = ConfigurationManager.AppSettings["Weee.CommandTimeout"];

            if (!string.IsNullOrEmpty(timeoutSettings))
            {
                int.TryParse(timeoutSettings, out cmdTimeout);
            }

            if (cmdTimeout >= 0)
            {
                context.Database.CommandTimeout = cmdTimeout;
            }
        }

        public async Task<List<EvidenceNoteReportData>> GetEvidenceNoteTonnagesReport(
            int complianceYear, Guid? recipientOrganisationId, Guid? aatfId, bool netTonnage)
        {
            var storedProcedure = netTonnage
                ? "[Evidence].[getEvidenceNotesNetTonnage]"
                : "[Evidence].[getEvidenceNotesOriginalTonnage]";

            var queryString = $"{storedProcedure} @ComplianceYear, @RecipientOrganisationId, @AatfId";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var pcsIdParameter = new SqlParameter("@RecipientOrganisationId", SqlDbType.UniqueIdentifier)
                {
                    IsNullable = true,
                    Value = recipientOrganisationId ?? (object)DBNull.Value
                };
            var aatfIdParameter = new SqlParameter("@AatfId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = aatfId ?? (object)DBNull.Value
            };

            return await context.Database.SqlQuery<EvidenceNoteReportData>(queryString, complianceYearParameter, pcsIdParameter, aatfIdParameter).ToListAsync();
        }

        public async Task<List<TransferNoteData>> GetTransferNoteReport(int complianceYear, Guid? organisationId)
        {
            const string storedProcedure = "[Evidence].[getTransferNotes]";

            var queryString = $"{storedProcedure} @ComplianceYear, @OrganisationId";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var organisationIdParameter = new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = organisationId ?? (object)DBNull.Value
            };
           
            return await context.Database.SqlQuery<TransferNoteData>(queryString, complianceYearParameter, organisationIdParameter).ToListAsync();
        }

        public async Task<List<AatfEvidenceSummaryTotalsData>> GetAatfEvidenceSummaryTotals(Guid aatfId, int complianceYear)
        {
            var queryString = "[Evidence].[getAatfEvidenceSummaryTotals] @AatfId, @ComplianceYear";
            var aatfIdParameter = new SqlParameter("@AatfId", aatfId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);

            return await context.Database.SqlQuery<AatfEvidenceSummaryTotalsData>(queryString, aatfIdParameter, complianceYearParameter).ToListAsync();
        }

        public async Task<List<ObligationEvidenceSummaryTotalsData>> GetObligationEvidenceSummaryTotals(Guid? pcsId, Guid? orgId, int complianceYear)
        {
            Condition.Requires(pcsId == null && orgId == null).IsFalse("pcsId and orgId cannot be both null");
            
            var queryString = "[Evidence].[getObligationEvidenceSummaryTotals] @ComplianceYear, @OrganisationId, @SchemeId ";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var orgIdParameter = new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier)
             {
                 IsNullable = true,
                 Value = orgId ?? (object)DBNull.Value
             };
            var pcsIdParameter = new SqlParameter("@SchemeId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = pcsId ?? (object)DBNull.Value
            };

            return await context.Database.SqlQuery<ObligationEvidenceSummaryTotalsData>(queryString, complianceYearParameter, orgIdParameter, pcsIdParameter).ToListAsync();
        }

        public async Task<List<ObligationAndEvidenceProgressSummaryData>> GetSchemeObligationAndEvidenceProgress(Guid? pcsId, Guid? appropriateAuthorityId, Guid? organisationId, int complianceYear)
        {
            var queryString = "[Evidence].[getSchemeObligationAndEvidenceTotals] @ComplianceYear, @SchemeId, @AppropriateAuthorityId, @OrganisationId ";

            var complianceYearParameter = new SqlParameter("@ComplianceYear", (short)complianceYear);
            var pcsIdParameter = new SqlParameter("@SchemeId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = pcsId ?? (object)DBNull.Value
            };
            var appropriateAuthorityIdParameter = new SqlParameter("@AppropriateAuthorityId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = appropriateAuthorityId ?? (object)DBNull.Value
            };
            var organisationIdParameter = new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier)
            {
                IsNullable = true,
                Value = organisationId ?? (object)DBNull.Value
            };

            return await context.Database.SqlQuery<ObligationAndEvidenceProgressSummaryData>(queryString, complianceYearParameter, pcsIdParameter, appropriateAuthorityIdParameter, organisationIdParameter).ToListAsync();
        }
    }
}
