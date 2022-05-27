namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using CuttingEdge.Conditions;
    using Domain.Error;
    using Domain.Obligation;
    using Domain.Security;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Shared;
    using Weee.Security;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, Guid>
    {
        private const string FileFormatError =
            "The error may be a problem with the file structure, which prevents our system from validating your file. You should rectify this error before we can continue our validation process.";

        private readonly IObligationCsvReader obligationCsvReader;
        private readonly IObligationUploadValidator obligationUploadValidator;
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess commonDataAccess;

        public SubmitSchemeObligationHandler(IObligationCsvReader obligationCsvReader, 
            IObligationUploadValidator obligationUploadValidator, 
            IWeeeAuthorization authorization, 
            ICommonDataAccess commonDataAccess)
        {
            this.obligationCsvReader = obligationCsvReader;
            this.obligationUploadValidator = obligationUploadValidator;
            this.authorization = authorization;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<Guid> HandleAsync(SubmitSchemeObligation request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var authority = await commonDataAccess.FetchCompetentAuthority(request.Authority);
            Condition.Requires(authority).IsNotNull();

            var errors = new List<ObligationUploadError>();
            var obligations = new List<ObligationCsvUpload>();

            try
            {
                obligations = obligationCsvReader.Read(request.FileInfo.Data).ToList();
            }
            catch (CsvValidationException)
            {
                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, FileFormatError));
            }
            catch (CsvReaderException)
            {
                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, FileFormatError));
            }

            var dataErrors = await obligationUploadValidator.Validate(obligations);

            errors.AddRange(dataErrors);
            
            return Guid.NewGuid();
        }
    }
}
