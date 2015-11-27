namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
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
        /// Gets all the member detail for specified compliance year for all schemes and all authorised authority.
        ///     If scheme Id is specified then filters on scheme Id
        ///     If AA Id is specified then filters on AA Id
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="schemeId"></param>
        /// <param name="competentAuthorityId"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(
            int complianceYear, Guid? schemeId = null, Guid? competentAuthorityId = null)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", (object)competentAuthorityId ?? DBNull.Value);

            return await context.Database
                .SqlQuery<MembersDetailsCSVData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @SchemeId, @CompetentAuthorityId",
                    complianceYearParameter,
                    schemeIdParameter,
                     competentAuthorityIdParameter)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all the PCS charges for the compliance year and specified AA
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="competentAuthorityId"></param>
        /// <returns></returns>
        public async Task<List<PCSChargesCSVData>> SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(int complianceYear, Guid? competentAuthorityId = null)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId", (object)competentAuthorityId ?? DBNull.Value);

            return await context.Database
                .SqlQuery<PCSChargesCSVData>(
                    "[Producer].[spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority] @ComplianceYear, @CompetentAuthorityId",
                    complianceYearParameter,
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
    }
}
