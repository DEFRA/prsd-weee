namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain;
    using Domain.DataReturns;
    using EA.Prsd.Core.Mediator;
    using Security;
    using Request = EA.Weee.Requests.DataReturns.FetchDataReturnForSubmission;

    public class FetchDataReturnForSubmissionHandler : IRequestHandler<Request, DataReturnForSubmission>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchDataReturnForSubmissionDataAccess dataAccess;

        public FetchDataReturnForSubmissionHandler(
            IWeeeAuthorization authorization,
            IFetchDataReturnForSubmissionDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnForSubmission> HandleAsync(Request message)
        {
            DataReturnUpload dataReturnsUpload = await dataAccess.FetchDataReturnUploadAsync(message.DataReturnUploadId);

            authorization.EnsureSchemeAccess(dataReturnsUpload.Scheme.Id);

            if (dataReturnsUpload.DataReturnVersion != null && dataReturnsUpload.DataReturnVersion.IsSubmitted)
            {
                string errorMessage = string.Format(
                    "The data return with ID \"{0}\" has already been submitted.",
                    dataReturnsUpload.Id);
                throw new InvalidOperationException(errorMessage);
            }

            List<ErrorLevel> errorLevelsWhichAreWarnings = new List<ErrorLevel>()
            {
                ErrorLevel.Debug,
                ErrorLevel.Info,
                ErrorLevel.Trace,
                ErrorLevel.Warning
            };

            List<ErrorLevel> errorLevelsWhichAreErrors = new List<ErrorLevel>()
            {
                ErrorLevel.Error,
                ErrorLevel.Fatal,
            };

            List<DataReturnWarning> warnings = dataReturnsUpload.Errors
                .Where(e => errorLevelsWhichAreWarnings.Contains(e.ErrorLevel))
                .Select(e => new DataReturnWarning(e.Description))
                .ToList();

            List<DataReturnError> errors = dataReturnsUpload.Errors
                .Where(e => errorLevelsWhichAreErrors.Contains(e.ErrorLevel))
                .Select(e => new DataReturnError(e.Description))
                .ToList();

            // TODO: Determine the correct quarter for the data return.
            Quarter quarter = new Quarter(2016, QuarterType.Q2);

            return new DataReturnForSubmission(
                dataReturnsUpload.Id,
                dataReturnsUpload.Scheme.OrganisationId,
                quarter,
                warnings,
                errors);
        }
    }
}
