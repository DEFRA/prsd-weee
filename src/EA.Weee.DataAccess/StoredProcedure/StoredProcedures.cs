namespace EA.Weee.DataAccess.StoredProcedure
{
    using Domain.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class StoredProcedures : IStoredProcedures
    {
        private readonly WeeeContext context;

        public StoredProcedures(WeeeContext context)
        {
            context.Database.CommandTimeout = 180;
            this.context = context;
        }

        public async Task<List<AatfSubmissionHistory>> GetAatfSubmissions(Guid aatfId, short complianceYear)
        {
            var aatfIdParameter = new SqlParameter("@AatfId", aatfId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<AatfSubmissionHistory>(
                    "[AATF].[getAatfSubmissions] @AatfId, @ComplianceYear",
                    aatfIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        public async Task<List<AatfSubmissionHistory>> GetAeSubmissions(Guid aatfId, short complianceYear)
        {
            var aatfIdParameter = new SqlParameter("@AeId", aatfId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<AatfSubmissionHistory>(
                    "[AATF].[getAeSubmissions] @AeId, @ComplianceYear",
                    aatfIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        public async Task<List<ProducerCsvData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId, int complianceYear)
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

            var schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            var competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", (object)competentAuthorityId ?? DBNull.Value);
            var includeRemovedProducerParameter = new SqlParameter("@IncludeRemovedProducer", includeRemovedProducer);
            var includeBrandNamesParameter = new SqlParameter("@IncludeBrandNames", includeBrandNames);

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
            var result = new SpgSchemeWeeeCsvResult();

            var command = context.Database.Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[PCS].[SpgSchemeWeeeCsv]";

            var complianceYearParameter = command.CreateParameter();
            complianceYearParameter.DbType = DbType.Int32;
            complianceYearParameter.Value = complianceYear;
            complianceYearParameter.ParameterName = "@ComplianceYear";
            command.Parameters.Add(complianceYearParameter);

            var schemeIdParameter = command.CreateParameter();
            schemeIdParameter.DbType = DbType.Guid;
            schemeIdParameter.Value = schemeId;
            schemeIdParameter.ParameterName = "@SchemeId";
            command.Parameters.Add(schemeIdParameter);

            var obligationTypeParameter = command.CreateParameter();
            obligationTypeParameter.DbType = DbType.String;
            obligationTypeParameter.Value = obligationType;
            obligationTypeParameter.ParameterName = "@ObligationType";
            command.Parameters.Add(obligationTypeParameter);

            command.CommandTimeout = 180;

            await context.Database.Connection.OpenAsync();

            var dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                var id = dataReader.GetGuid(dataReader.GetOrdinal("Id"));
                var schemeName = dataReader.GetString(dataReader.GetOrdinal("SchemeName"));
                var approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));

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
                var id = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                var quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                var weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                var sourceType = dataReader.GetInt32(dataReader.GetOrdinal("SourceType"));
                var tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

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
                var id = dataReader.GetGuid(dataReader.GetOrdinal("SchemeId"));
                var quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                var weeeCategory = dataReader.GetInt32(dataReader.GetOrdinal("WeeeCategory"));
                var locationType = dataReader.GetInt32(dataReader.GetOrdinal("LocationType"));
                var locationApprovalNumber = dataReader.GetString(dataReader.GetOrdinal("LocationApprovalNumber"));
                var tonnage = dataReader.GetDecimal(dataReader.GetOrdinal("Tonnage"));

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
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            var obligationTypeParameter = new SqlParameter("@ObligationType", obligationtype);

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

        public async Task<List<UkNonObligatedWeeeReceivedData>> GetUkNonObligatedWeeeReceivedByComplianceYear(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database.SqlQuery<UkNonObligatedWeeeReceivedData>("[AATF].[getUkNonObligatedWeeeReceivedByComplianceYear] @ComplianceYear",
                complianceYearParameter).ToListAsync();
        }

        public async Task<ProducerEeeHistoryCsvData> SpgProducerEeeHistoryCsvData(string prn)
        {
            var result = new ProducerEeeHistoryCsvData();

            var command = context.Database.Connection.CreateCommand();
            command.CommandTimeout = 180;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[Producer].[spgProducerEeeHistoryCsvDataByPRN]";

            var prnParameter = command.CreateParameter();
            prnParameter.DbType = DbType.String;
            prnParameter.Value = prn;
            prnParameter.ParameterName = "@PRN";
            command.Parameters.Add(prnParameter);

            await context.Database.Connection.OpenAsync();

            var dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                var prnValue = dataReader.GetString(dataReader.GetOrdinal("PRN"));
                var schemeName = dataReader.GetString(dataReader.GetOrdinal("SchemeName"));
                var approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));
                var year = dataReader.GetInt32(dataReader.GetOrdinal("ComplianceYear"));
                var quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                var date = dataReader.GetDateTime(dataReader.GetOrdinal("SubmittedDate"));
                var latest = dataReader.GetString(dataReader.GetOrdinal("LatestData"));
                //B2C categories
                var cat1b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT1B2C"));
                var cat2b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT2B2C"));
                var cat3b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT3B2C"));
                var cat4b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT4B2C"));
                var cat5b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT5B2C"));
                var cat6b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT6B2C"));
                var cat7b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT7B2C"));
                var cat8b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT8B2C"));
                var cat9b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT9B2C"));
                var cat10b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT10B2C"));
                var cat11b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT11B2C"));
                var cat12b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT12B2C"));
                var cat13b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT13B2C"));
                var cat14b2c = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT14B2C"));
                //B2B categories
                var cat1b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT1B2B"));
                var cat2b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT2B2B"));
                var cat3b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT3B2B"));
                var cat4b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT4B2B"));
                var cat5b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT5B2B"));
                var cat6b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT6B2B"));
                var cat7b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT7B2B"));
                var cat8b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT8B2B"));
                var cat9b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT9B2B"));
                var cat10b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT10B2B"));
                var cat11b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT11B2B"));
                var cat12b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT12B2B"));
                var cat13b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT13B2B"));
                var cat14b2b = GetDecimalValue(dataReader, dataReader.GetOrdinal("CAT14B2B"));

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
                var approvalNumber = dataReader.GetString(dataReader.GetOrdinal("ApprovalNumber"));
                var year = dataReader.GetInt32(dataReader.GetOrdinal("ComplianceYear"));
                var quarter = dataReader.GetInt32(dataReader.GetOrdinal("Quarter"));
                var date = dataReader.GetDateTime(dataReader.GetOrdinal("SubmittedDate"));

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
            var schemeIdParameter = new SqlParameter("@SchemeID", schemeId);
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            return await context.Database
                .SqlQuery<DataReturnSummaryCsvData>(
                    "[PCS].[spgDataReturnSummaryCsv] @SchemeID, @ComplianceYear",
                    schemeIdParameter,
                    complianceYearParameter)
                .ToListAsync();
        }

        public async Task<List<SchemeObligationCsvData>> SpgSchemeObligationDataCsv(int complianceYear)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

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
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var obligationTypeParameter = new SqlParameter("@ObligationType", obligationType);
            var quarterParameter = new SqlParameter("@Quarter", (object)quarter ?? DBNull.Value);
            var schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);

            return await context.Database
                .SqlQuery<MissingProducerDataCsvData>(
                    "[Producer].[spgMissingProducerDataCsvData] @ComplianceYear, @ObligationType, @Quarter, @SchemeId",
                    complianceYearParameter,
                    obligationTypeParameter,
                    quarterParameter,
                    schemeIdParameter)
                .ToListAsync();
        }

        public async Task<List<SubmissionChangesCsvData>> SpgSubmissionChangesCsvData(Guid memberUploadId)
        {
            var memberUploadIdParameter = new SqlParameter("@MemberUploadId", memberUploadId);

            return await context.Database
                .SqlQuery<SubmissionChangesCsvData>(
                    "[PCS].[spgSubmissionChangesCsvData] @MemberUploadId", memberUploadIdParameter)
                    .ToListAsync();
        }

        public async Task<List<AatfAeReturnData>> GetAatfAeReturnDataCsvData(int complianceYear, int quarter,
           int facilityType, int? returnStatus, Guid? authority, Guid? area, Guid? panArea, bool includeResubmissions)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var quarterParameter = new SqlParameter("@Quarter", quarter);
            var facilityTypeParameter = new SqlParameter("@FacilityType", facilityType);
            var returnStatusParameter = new SqlParameter("@ReturnStatus", (object)returnStatus ?? DBNull.Value);
            var authorityParameter = new SqlParameter("@CA", (object)authority ?? DBNull.Value);
            var areaParameter = new SqlParameter("@Area", (object)area ?? DBNull.Value);
            var panAreaParameter = new SqlParameter("@PanArea", (object)panArea ?? DBNull.Value);
            var includeResubmissionsParameter = new SqlParameter("@IncludeResubmissions", (object)includeResubmissions);

            return await context.Database
                .SqlQuery<AatfAeReturnData>(
                    "[AATF].[getAatfAeReturnDataCsvData] @ComplianceYear, @Quarter,  @FacilityType, @ReturnStatus, @CA, @Area, @PanArea, @IncludeResubmissions",
                    complianceYearParameter,
                    quarterParameter,
                    facilityTypeParameter,
                    returnStatusParameter,
                    authorityParameter,
                    areaParameter,
                    panAreaParameter,
                    includeResubmissionsParameter)
                .ToListAsync();
        }

        public async Task<List<NonObligatedWeeeReceivedCsvData>> GetNonObligatedWeeeReceivedAtAatf(int complianceYear, string aatf)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var aatfNameParameter = new SqlParameter("@AatfName", (object)aatf ?? DBNull.Value);

            return await context.Database
                .SqlQuery<NonObligatedWeeeReceivedCsvData>(
                    "[AATF].[getNonObligatedWeeeReceived] @ComplianceYear, @AatfName",
                    complianceYearParameter,
                    aatfNameParameter)
                .ToListAsync();
        }

        public async Task<DataTable> GetAllAatfObligatedCsvData(int complianceYear, string aatfName, string obligationType, Guid? authority, Guid? panArea, int columnType)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var aatfNameParameter = new SqlParameter("@AatfName", (object)aatfName ?? DBNull.Value);
            var obligationTypeParameter = new SqlParameter("@ObligationType", (object)obligationType ?? DBNull.Value);
            var authorityParameter = new SqlParameter("@CA", (object)authority ?? DBNull.Value);
            var panAreaParameter = new SqlParameter("@PanArea", (object)panArea ?? DBNull.Value);
            var columnTypeParameter = new SqlParameter("@ColumnType", columnType);

            var table = new DataTable();
            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[AATF].[getAllAatfObligatedCsvData]";
            cmd.Parameters.Add(complianceYearParameter);
            cmd.Parameters.Add(aatfNameParameter);
            cmd.Parameters.Add(obligationTypeParameter);
            cmd.Parameters.Add(authorityParameter);
            cmd.Parameters.Add(panAreaParameter);
            cmd.Parameters.Add(columnTypeParameter);

            cmd.CommandTimeout = 180;
            await cmd.Connection.OpenAsync();

            var res = await cmd.ExecuteReaderAsync();
            table.Load(res);

            return table;
        }

        public async Task<DataTable> GetAatfObligatedCsvData(Guid returnId, int complianceYear, int quarter, Guid aatfId)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var quarterParameter = new SqlParameter("@Quarter", quarter);
            var returnIdParameter = new SqlParameter("@ReturnId", returnId);
            var aatfIdParameter = new SqlParameter("@AatfId", aatfId);

            var table = new DataTable();

            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = "[AATF].[getAatfObligatedCsvData] @ComplianceYear, @Quarter, @ReturnId, @AatfId";
            cmd.Parameters.Add(complianceYearParameter);
            cmd.Parameters.Add(quarterParameter);
            cmd.Parameters.Add(returnIdParameter);
            cmd.Parameters.Add(aatfIdParameter);
            cmd.CommandTimeout = 180;
            await cmd.Connection.OpenAsync();
            table.Load(await cmd.ExecuteReaderAsync());

            return table;
        }

        public async Task<DataTable> GetReturnObligatedCsvData(Guid returnId)
        {
            var returnIdParameter = new SqlParameter("@ReturnId", returnId);

            var obligatedData = new DataTable();

            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = "[AATF].[getReturnObligatedCsvData] @ReturnId";
            cmd.Parameters.Add(returnIdParameter);
            cmd.CommandTimeout = 180;
            await cmd.Connection.OpenAsync();
            obligatedData.Load(await cmd.ExecuteReaderAsync());

            return obligatedData;
        }

        public async Task<List<NonObligatedWeeeReceivedCsvData>> GetReturnNonObligatedCsvData(Guid returnId)
        {
            var returnIdParameter = new SqlParameter("@ReturnId", returnId);

            return await context.Database.SqlQuery<NonObligatedWeeeReceivedCsvData>(
                "[AATF].[getReturnNonObligatedCsvData] @ReturnId",
                returnIdParameter).ToListAsync();
        }

        public async Task<DataSet> GetAllAatfSentOnDataCsv(int complianceYear, string obligationType, Guid? authority, Guid? panArea)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var obligationTypeParameter = new SqlParameter("@ObligationType", (object)obligationType ?? DBNull.Value);
            var authorityParameter = new SqlParameter("@CA", (object)authority ?? DBNull.Value);
            var panAreaParameter = new SqlParameter("@PanArea", (object)panArea ?? DBNull.Value);

            var dataSet = new DataSet();
            var sentOnData = new DataTable();
            var addressData = new DataTable();
            dataSet.Tables.Add(sentOnData);
            dataSet.Tables.Add(addressData);

            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = "[AATF].[getAllAatfSentOnCsvData] @ComplianceYear, @ObligationType, @CA, @PanArea";
            cmd.Parameters.Add(complianceYearParameter);
            cmd.Parameters.Add(obligationTypeParameter);
            cmd.Parameters.Add(authorityParameter);
            cmd.Parameters.Add(panAreaParameter);
            cmd.CommandTimeout = 180;
            await cmd.Connection.OpenAsync();
            dataSet.Load(await cmd.ExecuteReaderAsync(), LoadOption.OverwriteChanges, sentOnData, addressData);

            return dataSet;
        }

        public async Task<List<AatfReuseSitesData>> GetAllAatfReuseSitesCsvData(int complianceYear, Guid? authority, Guid? panArea)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var authorityParameter = new SqlParameter("@CA", (object)authority ?? DBNull.Value);
            var panAreaParameter = new SqlParameter("@PanArea", (object)panArea ?? DBNull.Value);

            return await context.Database
                .SqlQuery<AatfReuseSitesData>(
                    "[AATF].[getAllAatfReuseSitesCsvData] @ComplianceYear, @CA, @PanArea",
                    complianceYearParameter, authorityParameter, panAreaParameter)
                .ToListAsync();
        }

        public async Task<List<AatfAeDetailsData>> GetAatfAeDetailsCsvData(int complianceYear, int facilityType, Guid? authority, Guid? area, Guid? panArea)
        {
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            SqlParameter facilityTypeParameter = new SqlParameter("@FacilityType", facilityType);
            SqlParameter authorityParameter = new SqlParameter("@CA", (object)authority ?? DBNull.Value);
            SqlParameter areaParameter = new SqlParameter("@Area", (object)area ?? DBNull.Value);
            SqlParameter panAreaParameter = new SqlParameter("@PanArea", (object)panArea ?? DBNull.Value);

            return await context.Database
                .SqlQuery<AatfAeDetailsData>(
                    "[AATF].[getAatfAeDetailsCsvData] @ComplianceYear, @FacilityType, @CA, @Area, @PanArea",
                    complianceYearParameter, facilityTypeParameter, authorityParameter, areaParameter, panAreaParameter)
                .ToListAsync();
        }

        public async Task<List<PcsAatfComparisonDataCsvData>> GetPcsAatfComparisonDataCsvData(int complianceYear, int? quarter, string obligationType)
        {
            SqlParameter complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            SqlParameter quarterParameter = new SqlParameter("@Quarter", (object)quarter ?? 0);
            SqlParameter obligationTypeParameter = new SqlParameter("@ObligationType", (object)obligationType ?? DBNull.Value);

            return await context.Database
               .SqlQuery<PcsAatfComparisonDataCsvData>(
                   "[AATF].[getPcsAatfDiscrepancyCsvData] @ComplianceYear, @Quarter, @ObligationType",
                   complianceYearParameter, quarterParameter, obligationTypeParameter)
               .ToListAsync();
        }
    }
}
