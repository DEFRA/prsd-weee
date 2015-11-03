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
        ///     Gets all the member detail for specified compliance year for all schemes and all authorised authority.
        ///     If scheme name is specified then filters on scheme Name
        ///     If AA name is specified then filters on AA name
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="schmeName"></param>
        /// <param name="authorisedAuthorityName"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCsvData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(
            int complianceYear, string schmeName = "All", string authorisedAuthorityName = "All")
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);
            var schemeNameParameter = new SqlParameter("@SchemeName", schmeName);
            var authorisedAuthorityNameParameter = new SqlParameter("@AAName", authorisedAuthorityName);

            return await context.Database
                .SqlQuery<MembersDetailsCsvData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @SchemeName, @AAName",
                    complianceYearParameter,
                    schemeNameParameter,
                    authorisedAuthorityNameParameter)
                .ToListAsync();
        }
    }
}
