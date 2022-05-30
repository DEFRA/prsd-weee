namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
    using Domain;
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
        private readonly IObligationDataAccess obligationDataAccess;

        public SubmitSchemeObligationHandler(IObligationCsvReader obligationCsvReader, 
            IObligationUploadValidator obligationUploadValidator, 
            IWeeeAuthorization authorization, 
            ICommonDataAccess commonDataAccess, 
            IObligationDataAccess obligationDataAccess)
        {
            this.obligationCsvReader = obligationCsvReader;
            this.obligationUploadValidator = obligationUploadValidator;
            this.authorization = authorization;
            this.commonDataAccess = commonDataAccess;
            this.obligationDataAccess = obligationDataAccess;
        }

        public async Task<Guid> HandleAsync(SubmitSchemeObligation request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var authority = await commonDataAccess.FetchCompetentAuthority(request.Authority);
            Condition.Requires(authority).IsNotNull();

            var errors = new List<ObligationUploadError>();
            var obligations = new List<ObligationCsvUpload>();

            obligations = ReadCsv(request, errors);

            var dataErrors = await obligationUploadValidator.Validate(authority, obligations);

            var obligationUpload = await obligationDataAccess.AddObligationUpload(authority,
                System.Text.Encoding.UTF8.GetString(request.FileInfo.Data),
                request.FileInfo.FileName,
                errors);

            errors.AddRange(dataErrors);
            
            return obligationUpload;
        }

        private List<ObligationCsvUpload> ReadCsv(SubmitSchemeObligation request, ICollection<ObligationUploadError> errors)
        {
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

            return obligations;
        }
    }
}
