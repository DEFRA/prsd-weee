namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IFetchDataReturnForSubmissionDataAccess
    {
        Task<DataReturnsUpload> FetchDataReturnAsync(Guid dataReturnId);
    }
}
