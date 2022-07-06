namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Obligation;
    using Domain.Scheme;

    public interface IObligationDataAccess
    {
        Task<Guid> AddObligationUpload(UKCompetentAuthority ukCompetentAuthority,
            string data, 
            string fileName,
            IList<ObligationUploadError> errors,
            IList<ObligationScheme> obligations);

        Task<List<Scheme>> GetObligationScheme(UKCompetentAuthority authority, int complianceYear);

        Task<List<int>> GetObligationComplianceYears(UKCompetentAuthority authority);
    }
}