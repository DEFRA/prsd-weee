namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;

    public class FetchDataReturnForSubmissionDataAccess : IFetchDataReturnForSubmissionDataAccess
    {
        private readonly WeeeContext context;

        public FetchDataReturnForSubmissionDataAccess(WeeeContext dbContext)
        {
            this.context = dbContext;
        }
        public async Task<DataReturnUpload> FetchDataReturnUploadAsync(Guid dataReturnUploadId)
        {
            var result = await context
                .DataReturnsUploads
                //.Include(dru => dru.DataReturnsVersion)                
                .Include(dru => dru.Errors)
                .Where(dru => dru.Id == dataReturnUploadId)
                .SingleOrDefaultAsync();

            if (result == null)
            {
                string errorMessage = string.Format(
                    "A data return was not found with ID \"{0}\".",
                    dataReturnUploadId);
                throw new Exception(errorMessage);
            }

            return result;
        }
    }
}
