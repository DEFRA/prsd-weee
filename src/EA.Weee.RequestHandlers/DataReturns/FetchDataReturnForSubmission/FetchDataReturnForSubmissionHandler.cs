namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain;
    using Domain.DataReturns;
    using EA.Prsd.Core.Mediator;
    using Security;
    using Quarter = EA.Weee.Core.DataReturns.Quarter;
    using QuarterType = EA.Weee.Core.DataReturns.QuarterType;
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
            DataReturnsUpload dataReturn = await dataAccess.FetchDataReturnAsync(message.DataReturnId);

            authorization.EnsureSchemeAccess(dataReturn.Scheme.Id);

            if (dataReturn.IsSubmitted)
            {
                string errorMessage = string.Format(
                    "The data return with ID \"{0}\" has already been submitted.",
                    dataReturn.Id);
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

            List<DataReturnWarning> warnings = dataReturn.Errors
                .Where(e => errorLevelsWhichAreWarnings.Contains(e.ErrorLevel))
                .Select(e => new DataReturnWarning(e.Description))
                .ToList();

            List<DataReturnError> errors = dataReturn.Errors
                .Where(e => errorLevelsWhichAreErrors.Contains(e.ErrorLevel))
                .OrderBy(e => e.LineNumber)
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
