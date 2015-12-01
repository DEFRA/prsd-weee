namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFetchDataReturnForSubmissionDataAccess
    {
        Task<DataReturnsUpload> FetchDataReturnAsync(Guid dataReturnId);
    }
}
