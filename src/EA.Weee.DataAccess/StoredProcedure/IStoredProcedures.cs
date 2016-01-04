namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IStoredProcedures
    {
        Task<List<ProducerCSVData>> SpgCSVDataByOrganisationIdAndComplianceYear(Guid organisationId, int complianceYear);

        Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear);
        Task<List<MembersDetailsCSVData>> SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(int complianceYear, Guid? schemeId, Guid? competentAuthorityId);

        Task<int> SpgNext1B1STransactionNumber();

        //PCS charges
        Task<List<PCSChargesCSVData>> SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(int complianceYear);

        Task<List<PCSChargesCSVData>> SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(int complianceYear, Guid? competentAuthorityId);
        
        Task<List<ProducerPublicRegisterCSVData>> SpgProducerPublicRegisterCSVDataByComplianceYear(int complianceYear);

        Task<List<ProducerAmendmentsHistoryCSVData>> SpgProducerAmendmentsCSVDataByPRN(string prn);
    }
}
