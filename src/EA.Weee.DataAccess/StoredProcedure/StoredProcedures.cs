﻿namespace EA.Weee.DataAccess.StoredProcedure
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
        /// Gets all the member detail for specified compliance year for all schemes and all authorised authority.
        ///     If scheme Id is specified then filters on scheme Id
        ///     If AA Id is specified then filters on AA Id
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <param name="schemeId"></param>
        /// <param name="competentAuthorityId"></param>
        /// <returns></returns>
        public async Task<List<MembersDetailsCsvData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(
            int complianceYear, Guid? schemeId = null, Guid? competentAuthorityId = null)
        {
            var complianceYearParameter = new SqlParameter("@ComplianceYear", complianceYear);

            SqlParameter schemeIdParameter = new SqlParameter("@SchemeId", (object)schemeId ?? DBNull.Value);
            SqlParameter competentAuthorityIdParameter = new SqlParameter("@CompetentAuthorityId",  (object)competentAuthorityId ?? DBNull.Value);
            
            return await context.Database
                .SqlQuery<MembersDetailsCsvData>(
                    "[Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority] @ComplianceYear, @SchemeId, @CompetentAuthorityId",
                    complianceYearParameter,                                     
                    schemeIdParameter,
                     competentAuthorityIdParameter)
                .ToListAsync();
        }
    }
}
