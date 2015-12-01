namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchDataReturnForSubmissionDataAccess : IFetchDataReturnForSubmissionDataAccess
    {
        private WeeeContext context;

        public FetchDataReturnForSubmissionDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DataReturnsUpload> FetchDataReturnAsync(Guid dataReturnId)
        {
            var result = await context
                .DataReturnsUploads
                .Include(dru => dru.Errors)
                .Include(dru => dru.Scheme)
                .Where(dru => dru.Id == dataReturnId)
                .SingleOrDefaultAsync();

            if (result == null)
            {
                string errorMessage = string.Format(
                    "A data return was not found with ID \"{0}\".",
                    dataReturnId);
                throw new Exception(errorMessage);
            }

            return result;
        }
    }
}
