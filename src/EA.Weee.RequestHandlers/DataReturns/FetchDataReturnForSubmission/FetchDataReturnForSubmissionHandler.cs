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
    using QuarterType = EA.Weee.Core.DataReturns.QuarterType;
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
            DataReturnUpload dataReturnUpload = await dataAccess.FetchDataReturnUploadAsync(message.DataReturnUploadId);

            authorization.EnsureSchemeAccess(dataReturnUpload.Scheme.Id);

            if (dataReturnUpload.DataReturnVersion != null && dataReturnUpload.DataReturnVersion.IsSubmitted)
            {
                string errorMessage = string.Format(
                    "The data return with ID \"{0}\" has already been submitted.",
                    dataReturnUpload.Id);
                throw new InvalidOperationException(errorMessage);
            }

            List<DataReturnWarning> warnings = dataReturnUpload.Errors
                .Where(e => e.ErrorLevel == ErrorLevel.Warning)
                .Select(e => new DataReturnWarning(e.Description))
                .ToList();

            List<DataReturnError> errors = dataReturnUpload.Errors
                .Where(e => e.ErrorLevel == ErrorLevel.Error)
                .OrderBy(e => e.LineNumber)
                .Select(e => new DataReturnError(e.Description))
                .ToList();

            bool isResubmission;
            if (errors.Count == 0)
            {
                isResubmission = await dataAccess.CheckForExistingSubmissionAsync(
                    dataReturnUpload.Scheme.Id,
                    dataReturnUpload.ComplianceYear.Value,
                    dataReturnUpload.Quarter.Value);
            }
            else
            {
                isResubmission = false;
            }

            return new DataReturnForSubmission(
                dataReturnUpload.Id,
                dataReturnUpload.Scheme.OrganisationId,
                dataReturnUpload.ComplianceYear,
                (QuarterType?)dataReturnUpload.Quarter,
                warnings,
                errors,
                isResubmission);
        }
    }
}
