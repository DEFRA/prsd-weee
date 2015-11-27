namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public interface IStoredProcedures
    {
        Task<List<ProducerCSVData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId, int complianceYear);

        Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, Guid? schemeId = null, Guid? competentAuthorityId = null);

        Task<List<PCSChargesCSVData>> SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(int complianceYear, Guid? competentAuthorityId = null);

        Task<List<ProducerPublicRegisterCSVData>> SpgProducerPublicRegisterCSVDataByComplianceYear(int complianceYear);
    }
}
