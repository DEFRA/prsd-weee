namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Obligation;

    public interface IObligationDataAccess
    {
        Task<Guid> AddObligationUpload(UKCompetentAuthority ukCompetentAuthority,
            string data, 
            string fileName,
            IList<ObligationUploadError> errors);
    }
}