namespace EA.Weee.RequestHandlers.DataReturns.Upload
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class FetchDataReturnUploadDataAccess : IFetchDataReturnUploadDataAccess
    {
        private readonly WeeeContext context;

        public FetchDataReturnUploadDataAccess(WeeeContext dbContext)
        {
            this.context = dbContext;
        }

        public async Task<DataReturnUpload> FetchDataReturnUploadByIdAsync(Guid dataReturnUploadId)
        {
            return await context.DataReturnsUploads.Where(dru => dru.Id == dataReturnUploadId).SingleOrDefaultAsync();
        }
    }
}
