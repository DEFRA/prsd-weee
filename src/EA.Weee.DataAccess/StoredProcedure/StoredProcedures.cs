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

        public async Task<List<ProducerCsvData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId,
            int complianceYear)
        {
            var organisationIdParameter = new SqlParameter("@OrganisationId", organisationId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<ProducerCsvData>(
                    "[Producer].[spgCSVDataByOrganisationIdAndComplianceYear] @OrganisationId, @ComplianceYear",
                    organisationIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all the member detail for specified compliance year for all schemes and all authorised authority.
        ///     If scheme Id is specified then filters on scheme Id
        ///     If AA Id is specified then filters on AA Id
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="includeRemovedProducer"></param>
        /// <param name="includeBrandNames"></param>
        /// <param name="schemeId"></param>
        /// <param name="competentAuthorityId"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCsvData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, bool includeRemovedProducer,
            bool includeBrandNames, Guid? schemeId, Guid? competentAuthorityId)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", (object)competentAuthorityId ?? DBNull.Value);
            SqlParameter includeRemovedProducerParameter = new SqlParameter("@IncludeRemovedProducer", includeRemovedProducer);
            SqlParameter includeBrandNamesParameter = new SqlParameter("@IncludeBrandNames", includeBrandNames);

            return await context.Database
                .SqlQuery<MembersDetailsCsvData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @IncludeRemovedProducer, @IncludeBrandNames, @SchemeId, @CompetentAuthorityId",
                    complianceYearParameter,
                    includeRemovedProducerParameter,
                    includeBrandNamesParameter,
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

        public async Task<SpgSchemeWeeeCsvResult> SpgSchemeWeeeCsvAsync(int complianceYear, Guid? schemeId, string obligationType)
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

            DbParameter schemeIdParameter = command.CreateParameter();
            schemeIdParameter.DbType = System.Data.DbType.Guid;
            schemeIdParameter.Value = schemeId;
            schemeIdParameter.ParameterName = "@SchemeId";
            command.Parameters.Add(schemeIdParameter);

            DbParameter obligationTypeParameter = command.CreateParameter();
            obligationTypeParameter.DbType = System.Data.DbType.String;
            obligationTypeParameter.Value = obligationType;
            obligationTypeParameter.ParameterName = "@ObligationType";
            command.Parameters.Add(obligationTypeParameter);

            await context.Database.Connection.OpenAsync();

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                Guid id = dataReader.GetGuid(dataReader.GetOrdinal("Id"));
                string schemeName = dataReader.GetString(dataReader.GetOrdinal("SchemeName"));
                string approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));

                result.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
                {
                    SchemeId = id,
                    SchemeName = schemeName,
                    ApprovalNumber = approvalNumber
                });
            }

            await dataReader.NextResultAsync();

            while (await dataReader.ReadAsync())
            {
                Guid id = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                int weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                int sourceType = dataReader.GetInt32(dataReader.GetOrdinal("SourceType"));
                decimal tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

                result.CollectedAmounts.Add(new SpgSchemeWeeeCsvResult.CollectedAmountResult()
                {
                    SchemeId = id,
                    QuarterType = quarter,
                    WeeeCategory = weeeCategory,
                    SourceType = sourceType,
                    Tonnage = tonnage
                });
            }

            await dataReader.NextResultAsync();

            while (await dataReader.ReadAsync())
            {
                Guid id = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                int weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                int locationType = dataReader.GetInt32(dataReader.GetOrdinal("LocationType"));
                string locationApprovalNumber = dataReader.GetString(dataReader.GetOrdinal("LocationApprovalNumber"));
                decimal tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

                result.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
                {
                    SchemeId = id,
                    QuarterType = quarter,
                    WeeeCategory = weeeCategory,
                    LocationType = locationType,
                    LocationApprovalNumber = locationApprovalNumber,
                    Tonnage = tonnage
                });
            }

            return result;
        }

        public async Task<List<ProducerEeeCsvData>> SpgProducerEeeCsvData(int complianceYear, Guid? schemeId, string obligationtype)
        {
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            SqlParameter obligationTypeParameter = new SqlParameter("@ObligationType", obligationtype);

            return await context.Database
                .SqlQuery<ProducerEeeCsvData>("[Producer].[spgProducerEeeCsvData] @ComplianceYear, @SchemeId, @ObligationType",
                    complianceYearParameter,
                    schemeIdParameter,
                    obligationTypeParameter)
                .ToListAsync();
        }

        public async Task<List<UkEeeCsvData>> SpgUKEEEDataByComplianceYear(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<UkEeeCsvData>(
                    "[Producer].[SpgUKEEEDataByComplianceYear] @ComplianceYear",
                    complianceYearParameter).ToListAsync();
        }

        public async Task<ProducerEeeHistoryCsvData> SpgProducerEeeHistoryCsvData(string prn)
        {
            ProducerEeeHistoryCsvData result = new ProducerEeeHistoryCsvData();

            var command = context.Database.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "[Producer].[spgProducerEeeHistoryCsvDataByPRN]";

            DbParameter prnParameter = command.CreateParameter();
            prnParameter.DbType = System.Data.DbType.String;
            prnParameter.Value = prn;
            prnParameter.ParameterName = "@PRN";
            command.Parameters.Add(prnParameter);

            await context.Database.Connection.OpenAsync();

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                string prnValue = dataReader.GetString(dataReader.GetOrdinal("PRN"));
                string schemeName = dataReader.GetString(dataReader.GetOrdinal("SchemeName"));
                string approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));
                int year = dataReader.GetInt32(dataReader.GetOrdinal("ComplianceYear"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                DateTime date = dataReader.GetDateTime(dataReader.GetOrdinal("SubmittedDate"));
                string latest = dataReader.GetString(dataReader.GetOrdinal("LatestData"));
                //B2C categories
                decimal? cat1b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT1B2C"));
                decimal? cat2b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT2B2C"));
                decimal? cat3b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT3B2C"));
                decimal? cat4b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT4B2C"));
                decimal? cat5b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT5B2C"));
                decimal? cat6b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT6B2C"));
                decimal? cat7b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT7B2C"));
                decimal? cat8b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT8B2C"));
                decimal? cat9b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT9B2C"));
                decimal? cat10b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT10B2C"));
                decimal? cat11b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT11B2C"));
                decimal? cat12b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT12B2C"));
                decimal? cat13b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT13B2C"));
                decimal? cat14b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT14B2C"));
                //B2B categories
                decimal? cat1b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT1B2B"));
                decimal? cat2b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT2B2B"));
                decimal? cat3b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT3B2B"));
                decimal? cat4b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT4B2B"));
                decimal? cat5b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT5B2B"));
                decimal? cat6b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT6B2B"));
                decimal? cat7b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT7B2B"));
                decimal? cat8b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT8B2B"));
                decimal? cat9b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT9B2B"));
                decimal? cat10b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT10B2B"));
                decimal? cat11b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT11B2B"));
                decimal? cat12b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT12B2B"));
                decimal? cat13b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT13B2B"));
                decimal? cat14b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT14B2B"));

                result.ProducerReturnsHistoryData.Add(new ProducerEeeHistoryCsvData.ProducerInReturnsResult()
                {
                    PRN = prnValue,
                    SchemeName = schemeName,
                    ApprovalNumber = approvalNumber,
                    ComplianceYear = year,
                    SubmittedDate = date,
                    Quarter = quarter,
                    LatestData = latest,
                    Cat1B2C = cat1b2c,
                    Cat2B2C = cat2b2c,
                    Cat3B2C = cat3b2c,
                    Cat4B2C = cat4b2c,
                    Cat5B2C = cat5b2c,
                    Cat6B2C = cat6b2c,
                    Cat7B2C = cat7b2c,
                    Cat8B2C = cat8b2c,
                    Cat9B2C = cat9b2c,
                    Cat10B2C = cat10b2c,
                    Cat11B2C = cat11b2c,
                    Cat12B2C = cat12b2c,
                    Cat13B2C = cat13b2c,
                    Cat14B2C = cat14b2c,
                    Cat1B2B = cat1b2b,
                    Cat2B2B = cat2b2b,
                    Cat3B2B = cat3b2b,
                    Cat4B2B = cat4b2b,
                    Cat5B2B = cat5b2b,
                    Cat6B2B = cat6b2b,
                    Cat7B2B = cat7b2b,
                    Cat8B2B = cat8b2b,
                    Cat9B2B = cat9b2b,
                    Cat10B2B = cat10b2b,
                    Cat11B2B = cat11b2b,
                    Cat12B2B = cat12b2b,
                    Cat13B2B = cat13b2b,
                    Cat14B2B = cat14b2b
                });
            }

            await dataReader.NextResultAsync();

            while (await dataReader.ReadAsync())
            {
                string approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));
                int year = dataReader.GetInt32(dataReader.GetOrdinal("ComplianceYear"));
                int quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                DateTime date = dataReader.GetDateTime(dataReader.GetOrdinal("SubmittedDate"));

                result.ProducerRemovedFromReturnsData.Add(new ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult()
                {
                    ApprovalNumber = approvalNumber,
                    ComplianceYear = year,
                    SubmittedDate = date,
                    Quarter = quarter
                });
            }
            return result;
        }

        private static decimal? GetDecimalValue(DbDataReader dataReader, int index)
        {
            decimal? value = null;
            if (!dataReader.IsDBNull(index))
            {
                value = dataReader.GetDecimal(index);
            }
            return value;
        }

        public async Task<List<DataReturnSummaryCsvData>> SpgDataReturnSummaryCsv(Guid schemeId, int complianceYear)
        {
            SqlParameter schemeIdParameter = new SqlParameter("@SchemeID", schemeId);
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<DataReturnSummaryCsvData>(
                    "[PCS].[spgDataReturnSummaryCsv] @SchemeID, @ComplianceYear",
                    schemeIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        public async Task<List<SchemeObligationCsvData>> SpgSchemeObligationDataCsv(int complianceYear)
        {
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<SchemeObligationCsvData>(
                    "[Producer].[spgSchemeObligationCsvData] @ComplianceYear",
                    complianceYearParameter)
                .ToListAsync();
        }

        public async Task<List<MissingProducerDataCsvData>> SpgMissingProducerDataCsvData(int complianceYear,
            string obligationType,
            int? quarter,
            Guid? schemeId)
        {
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            SqlParameter obligationTypeParameter = new SqlParameter("@ObligationType", obligationType);
            SqlParameter quarterParameter = new SqlParameter("@Quarter", (object)quarter ?? DBNull.Value);
            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);

            return await context.Database
                .SqlQuery<MissingProducerDataCsvData>(
                    "[Producer].[spgMissingProducerDataCsvData] @ComplianceYear, @ObligationType, @Quarter, @SchemeId",
                    complianceYearParameter,
                    obligationTypeParameter,
                    quarterParameter,
                    schemeIdParameter)
                .ToListAsync();
        }
    }
}
