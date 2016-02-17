namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class FetchDataReturnForSubmissionDataAccess : IFetchDataReturnForSubmissionDataAccess
    {
        private readonly WeeeContext context;

        public FetchDataReturnForSubmissionDataAccess(WeeeContext dbContext)
        {
            this.context = dbContext;
        }

        public async Task<DataReturnUpload> FetchDataReturnUploadAsync(Guid dataReturnUploadId)
        {
            DataReturnUpload result = await context
                .DataReturnsUploads
                .Include(dru => dru.DataReturnVersion)                
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

        public async Task<DataReturnUpload> FetchDataReturnUploadByIdAsync(Guid dataReturnUploadId)
        {
            return await context.DataReturnsUploads.Where(dru => dru.Id == dataReturnUploadId).SingleOrDefaultAsync();
        }

        public async Task<bool> CheckForExistingSubmissionAsync(Guid schemeId, int complianceYear, int quarterType)
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme.Id == schemeId)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .Where(dr => (int)dr.Quarter.Q == quarterType)
                .Where(dr => dr.CurrentVersion != null)
                .AnyAsync();
        }
    }
}
