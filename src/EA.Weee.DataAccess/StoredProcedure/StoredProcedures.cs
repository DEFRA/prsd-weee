namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class StoredProcedures : IStoredProcedures
    {
        private readonly WeeeContext context;

        public StoredProcedures(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<ProducerCSVData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId,
            int complianceYear)
        {
            var organisationIdParameter = new SqlParameter("@OrganisationId", organisationId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<ProducerCSVData>(
                    "[Producer].[spgCSVDataByOrganisationIdAndComplianceYear] @OrganisationId, @ComplianceYear",
                    organisationIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        /// <summary>
        /// /Gets all the member detail for specified compliance year
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", DBNull.Value);
            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", DBNull.Value);
            SqlParameter includeRemovedProducerParameter = new SqlParameter("@IncludeRemovedProducer", false);

            return await context.Database
                .SqlQuery<MembersDetailsCSVData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @IncludeRemovedProducer, @SchemeId, @CompetentAuthorityId",
                    complianceYearParameter,
                     includeRemovedProducerParameter,
                    schemeIdParameter,
                     competentAuthorityIdParameter)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all the member detail for specified compliance year for all schemes and all authorised authority.
        ///     If scheme Id is specified then filters on scheme Id
        ///     If AA Id is specified then filters on AA Id
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="schemeId"></param>
        /// <param name="competentAuthorityId"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, bool includeRemovedProducer, Guid? schemeId, Guid? competentAuthorityId)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", (object)competentAuthorityId ?? DBNull.Value);
            SqlParameter includeRemovedProducerParameter = new SqlParameter("@IncludeRemovedProducer", includeRemovedProducer);

            return await context.Database
                .SqlQuery<MembersDetailsCSVData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @IncludeRemovedProducer, @SchemeId, @CompetentAuthorityId",
                    complianceYearParameter,
                     includeRemovedProducerParameter,
                    schemeIdParameter,
                     competentAuthorityIdParameter)
                .ToListAsync();
        }

        /// <summary>
        /// Gets Producer public register data by compliance year
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        public async Task<List<ProducerPublicRegisterCSVData>> SpgProducerPublicRegisterCSVDataByComplianceYear(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<ProducerPublicRegisterCSVData>(
                    "[Producer].[spgProducerPublicRegisterCSVDataByComplianceYear] @ComplianceYear",
                    complianceYearParameter).ToListAsync();
        }

        /// <summary>
        /// Gets producer amendments history
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        public async Task<List<ProducerAmendmentsHistoryCSVData>> SpgProducerAmendmentsCSVDataByPRN(string prn)
        {
            var prnParameter = new SqlParameter("@PRN", prn);

            return await context.Database
                .SqlQuery<ProducerAmendmentsHistoryCSVData>(
                    "[Producer].[spgProducerAmendmentsCSVDataByPRN] @PRN",
                    prnParameter).ToListAsync();
        }

        public async Task<int> SpgNextIbisTransactionNumber()
        {
            return await context.Database
                .SqlQuery<int>("[Charging].[SpgNextIbisTransactionNumber]")
                .SingleAsync();
        }

        /// <summary>
        /// Gets the details of the producers for an invoice run.
        /// </summary>
        /// <param name="invoiceRunId"></param>
        /// <returns></returns>
        public async Task<List<PCSChargesCSVData>> SpgInvoiceRunChargeBreakdown(Guid invoiceRunId)
        {
            var invoiceRunIdParameter = new SqlParameter("@InvoiceRunId", invoiceRunId);

            return await context.Database
                .SqlQuery<PCSChargesCSVData>(
                    "[Charging].[SpgInvoiceRunChargeBreakdown] @InvoiceRunId",
                    invoiceRunIdParameter)
                .ToListAsync();
        }

        public async Task<SpgSchemeWeeeCsvResult> SpgSchemeWeeeCsvAsync(int complianceYear, string obligationType)
        {
            SpgSchemeWeeeCsvResult result = new SpgSchemeWeeeCsvResult();

            var command = context.Database.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "[PCS].[SpgSchemeWeeeCsv]";

            DbParameter complianceYearParameter = command.CreateParameter();
            complianceYearParameter.DbType = System.Data.DbType.Int32;
            complianceYearParameter.Value = complianceYear;
            complianceYearParameter.ParameterName = "@ComplianceYear";
            command.Parameters.Add(complianceYearParameter);

            DbParameter obligationTypeParameter = command.CreateParameter();
            obligationTypeParameter.DbType = System.Data.DbType.String;
            obligationTypeParameter.Value = obligationType;
            obligationTypeParameter.ParameterName = "@ObligationType";
            command.Parameters.Add(obligationTypeParameter);

            await context.Database.Connection.OpenAsync();

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                Guid schemeId = dataReader.GetGuid(dataReader.GetOrdinal("Id"));
                string schemeName = dataReader.GetString(dataReader.GetOrdinal("SchemeName"));
                string approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));

                result.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
                {
                    SchemeId = schemeId,
                    SchemeName = schemeName,
                    ApprovalNumber = approvalNumber
                });
            }

            await dataReader.NextResultAsync();

            while (await dataReader.ReadAsync())
            {
                Guid schemeId = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                int weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                int sourceType = dataReader.GetInt32(dataReader.GetOrdinal("SourceType"));
                decimal tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

                result.CollectedAmounts.Add(new SpgSchemeWeeeCsvResult.CollectedAmountResult()
                {
                    SchemeId = schemeId,
                    QuarterType = quarter,
                    WeeeCategory = weeeCategory,
                    SourceType = sourceType,
                    Tonnage = tonnage
                });
            }

            await dataReader.NextResultAsync();

            while (await dataReader.ReadAsync())
            {
                Guid schemeId = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                int weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                int locationType = dataReader.GetInt32(dataReader.GetOrdinal("LocationType"));
                string locationApprovalNumber = dataReader.GetString(dataReader.GetOrdinal("LocationApprovalNumber"));
                decimal tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

                result.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
                {
                    SchemeId = schemeId,
                    QuarterType = quarter,
                    WeeeCategory = weeeCategory,
                    LocationType = locationType,
                    LocationApprovalNumber = locationApprovalNumber,
                    Tonnage = tonnage
                });
            }

            return result;
        }

        public async Task<List<ProducerEeeCsvData>> SpgProducerEeeCsvDataByComplianceYearAndObligationType(int complianceYear, string obligationtype)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            SqlParameter obligationTypeParameter = new SqlParameter("@ObligationType", obligationtype);
            return await context.Database
                .SqlQuery<ProducerEeeCsvData>(
                    "[Producer].[spgProducerEEECSVDataByComplianceYearAndObligationType] @ComplianceYear, @ObligationType",
                    complianceYearParameter,
                    obligationTypeParameter).ToListAsync();
        }

        public async Task<List<UkEeeCsvData>> SpgUKEEEDataByComplianceYear(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<UkEeeCsvData>(
                    "[Producer].[SpgUKEEEDataByComplianceYear] @ComplianceYear",
                    complianceYearParameter).ToListAsync();
        }

        public async Task<List<ProducerEeeHistoryCsvData>> SpgProducerEeeHistoryCsvData(string prn)
        {
            var prnParameter = new SqlParameter("@PRN", prn);

            return await context.Database
                .SqlQuery<ProducerEeeHistoryCsvData>(
                    "[Producer].[SpgProducerEeeHistoryCsvDataByPRN] @PRN",
                    prnParameter).ToListAsync();
        }
    }
}
