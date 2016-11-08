namespace EA.Weee.RequestHandlers.Admin.GetDataReturnSubmissionChanges
{
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;
    using Requests.Admin.GetDataReturnSubmissionChanges;
    using Security;

    public class GetDataReturnSubmissionEeeChangesCsvHandler : IRequestHandler<GetDataReturnSubmissionEeeChangesCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnSubmissionEeeChangesCsvDataAccess dataAccess;
        private readonly ICsvWriter<DataReturnSubmissionEeeChangesCsvData> csvWriter;

        public GetDataReturnSubmissionEeeChangesCsvHandler(
            IWeeeAuthorization authorization,
            IGetDataReturnSubmissionEeeChangesCsvDataAccess dataAccess,
            ICsvWriter<DataReturnSubmissionEeeChangesCsvData> csvWriter)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.csvWriter = csvWriter;
        }

        public async Task<CSVFileData> HandleAsync(GetDataReturnSubmissionEeeChangesCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            var changes = await dataAccess.GetChanges(message.CurrentDataReturnVersionId, message.PreviousDataReturnVersionId);

            csvWriter.DefineColumn("Producer name", i => i.ProducerName);
            csvWriter.DefineColumn("PRN", i => i.ProducerRegistrationNumber);
            csvWriter.DefineColumn("Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn("Date / Time (GMT) of submission", i => i.SubmissionDate);
            csvWriter.DefineColumn("Quarter", i => i.Quarter);
            csvWriter.DefineColumn("Change type", i => i.ChangeType);

            csvWriter.DefineColumn("Cat1 B2C (t)", i => i.Cat1B2C);
            csvWriter.DefineColumn("Cat2 B2C (t)", i => i.Cat2B2C);
            csvWriter.DefineColumn("Cat3 B2C (t)", i => i.Cat3B2C);
            csvWriter.DefineColumn("Cat4 B2C (t)", i => i.Cat4B2C);
            csvWriter.DefineColumn("Cat5 B2C (t)", i => i.Cat5B2C);
            csvWriter.DefineColumn("Cat6 B2C (t)", i => i.Cat6B2C);
            csvWriter.DefineColumn("Cat7 B2C (t)", i => i.Cat7B2C);
            csvWriter.DefineColumn("Cat8 B2C (t)", i => i.Cat8B2C);
            csvWriter.DefineColumn("Cat9 B2C (t)", i => i.Cat9B2C);
            csvWriter.DefineColumn("Cat10 B2C (t)", i => i.Cat10B2C);
            csvWriter.DefineColumn("Cat11 B2C (t)", i => i.Cat11B2C);
            csvWriter.DefineColumn("Cat12 B2C (t)", i => i.Cat12B2C);
            csvWriter.DefineColumn("Cat13 B2C (t)", i => i.Cat13B2C);
            csvWriter.DefineColumn("Cat14 B2C (t)", i => i.Cat14B2C);

            csvWriter.DefineColumn("Cat1 B2B (t)", i => i.Cat1B2B);
            csvWriter.DefineColumn("Cat2 B2B (t)", i => i.Cat2B2B);
            csvWriter.DefineColumn("Cat3 B2B (t)", i => i.Cat3B2B);
            csvWriter.DefineColumn("Cat4 B2B (t)", i => i.Cat4B2B);
            csvWriter.DefineColumn("Cat5 B2B (t)", i => i.Cat5B2B);
            csvWriter.DefineColumn("Cat6 B2B (t)", i => i.Cat6B2B);
            csvWriter.DefineColumn("Cat7 B2B (t)", i => i.Cat7B2B);
            csvWriter.DefineColumn("Cat8 B2B (t)", i => i.Cat8B2B);
            csvWriter.DefineColumn("Cat9 B2B (t)", i => i.Cat9B2B);
            csvWriter.DefineColumn("Cat10 B2B (t)", i => i.Cat10B2B);
            csvWriter.DefineColumn("Cat11 B2B (t)", i => i.Cat11B2B);
            csvWriter.DefineColumn("Cat12 B2B (t)", i => i.Cat12B2B);
            csvWriter.DefineColumn("Cat13 B2B (t)", i => i.Cat13B2B);
            csvWriter.DefineColumn("Cat14 B2B (t)", i => i.Cat14B2B);

            var fileContent = csvWriter.Write(changes.CsvData);

            var fileName =
                string.Format("{0}_Q{1}_{2}_EEEDataChanges_{3:ddMMyyyy_HHmm}.csv",
                changes.ComplianceYear, changes.Quarter, changes.SchemeApprovalNumber,
                changes.CurrentSubmissionDate);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
