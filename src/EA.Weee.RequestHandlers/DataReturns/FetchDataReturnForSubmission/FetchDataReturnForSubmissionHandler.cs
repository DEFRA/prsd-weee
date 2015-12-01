namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.DataReturns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Request = EA.Weee.Requests.DataReturns.FetchDataReturnForSubmission;

    public class FetchDataReturnForSubmissionHandler : IRequestHandler<Request, DataReturnForSubmission>
    {
        private IWeeeAuthorization authorization;
        private IFetchDataReturnForSubmissionDataAccess dataAccess;

        public FetchDataReturnForSubmissionHandler(
            IWeeeAuthorization authorization,
            IFetchDataReturnForSubmissionDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnForSubmission> HandleAsync(Request message)
        {
            Domain.Scheme.DataReturnsUpload dataReturn = await dataAccess.FetchDataReturnAsync(message.DataReturnId);

            authorization.EnsureSchemeAccess(dataReturn.SchemeId);

            if (dataReturn.IsSubmitted)
            {
                string errorMessage = string.Format(
                    "The data return with ID \"{0}\" has already been submitted.",
                    dataReturn.Id);
                throw new InvalidOperationException(errorMessage);
            }

            List<DataReturnWarning> warnings = dataReturn.Errors
                .Where(e => e.ErrorLevel == ErrorLevel.Warning)
                .Select(e => new DataReturnWarning(e.Description))
                .ToList();

            List<DataReturnError> errors = dataReturn.Errors
                .Where(e => e.ErrorLevel == ErrorLevel.Error)
                .Select(e => new DataReturnError(e.Description))
                .ToList();

            // TODO: Determine the correct quarter for the data return.
            Quarter quarter = new Quarter(2016, QuarterType.Q2);

            return new DataReturnForSubmission(
                dataReturn.Id,
                dataReturn.Scheme.OrganisationId,
                quarter,
                warnings,
                errors);
        }
    }
}
