namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, Guid>
    {
        private readonly IFileHelper fileHelper;
        private readonly IObligationCsvReader obligationCsvReader;

        public SubmitSchemeObligationHandler(IFileHelper fileHelper, 
            IObligationCsvReader obligationCsvReader)
        {
            this.fileHelper = fileHelper;
            this.obligationCsvReader = obligationCsvReader;
        }

        public Task<Guid> HandleAsync(SubmitSchemeObligation request)
        {
            obligationCsvReader.ValidateHeader(request.FileInfo.Data);

            return Task.FromResult(Guid.NewGuid());
        }
    }
}
