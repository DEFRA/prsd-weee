namespace EA.Weee.DataAccess.DataAccess
{
    using Domain;
    using Domain.Obligation;
    using Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IObligationDataAccess
    {
        Task<Guid> AddObligationUpload(UKCompetentAuthority ukCompetentAuthority,
            string data, 
            string fileName,
            IList<ObligationUploadError> errors,
            IList<ObligationScheme> obligations);

        Task<List<Scheme>> GetObligationSchemeData(UKCompetentAuthority authority, int complianceYear);

        Task<List<int>> GetObligationComplianceYears(UKCompetentAuthority authority);

        Task<List<Scheme>> GetSchemesWithObligationOrEvidence(int complianceYear);
    }
}