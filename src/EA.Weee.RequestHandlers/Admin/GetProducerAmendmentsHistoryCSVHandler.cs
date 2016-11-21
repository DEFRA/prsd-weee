namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    
    internal class GetProducerAmendmentsHistoryCSVHandler : IRequestHandler<GetProducerAmendmentsHistoryCSV, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public const short MaxBrandNamesLength = short.MaxValue;

        public GetProducerAmendmentsHistoryCSVHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducerAmendmentsHistoryCSV request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (string.IsNullOrEmpty(request.PRN))
            {
                throw new ArgumentException("PRN is required.");
            }

            var items = await context.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN(request.PRN);

            CsvWriter<ProducerAmendmentsHistoryCSVData> csvWriter = csvWriterFactory.Create<ProducerAmendmentsHistoryCSVData>();
            csvWriter.DefineColumn(@"Compliance Year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Date & time (GMT) registered", i => (i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss").Equals(i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")) ? i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty));
            csvWriter.DefineColumn(@"Date & time (GMT) updated", i => (i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss").Equals(i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")) ? string.Empty : i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")));
            csvWriter.DefineColumn(@"PCS name", i => i.PCSName);
            csvWriter.DefineColumn(@"PCS approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Producer name", i => i.ProducerName);
            csvWriter.DefineColumn(@"Producer type", i => i.ProducerType);
            csvWriter.DefineColumn(@"Company registration number", i => i.CompanyNumber);
            csvWriter.DefineColumn(@"Partnership names", i => i.Partners);
            csvWriter.DefineColumn(@"Trading name", i => i.TradingName);
            csvWriter.DefineColumn(@"PRN", i => i.PRN);

            csvWriter.DefineColumn(@"Charge band", i => i.ChargeBandType);
            csvWriter.DefineColumn(@"VAT registered", (i => i.VATRegistered ? "Yes" : "No"));
            csvWriter.DefineColumn(@"Annual turnover", i => i.AnnualTurnover.HasValue ? i.AnnualTurnover.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            csvWriter.DefineColumn(@"Annual turnover band", i => i.AnnualTurnoverBandType);
            csvWriter.DefineColumn(@"EEE placed on market", i => i.EEEPlacedOnMarketBandType);
            csvWriter.DefineColumn(@"Obligation Type", i => i.ObligationType);
            csvWriter.DefineColumn(@"SIC codes", i => i.SICCodes);
            csvWriter.DefineColumn(@"Selling technique", i => i.SellingTechniqueType);
            csvWriter.DefineColumn(@"Date ceased to exist", i => i.CeaseToExist.HasValue ? i.CeaseToExist.Value.ToString("dd/MM/yyyy") : string.Empty);

            //correspondences of notices details
            csvWriter.DefineColumn(@"Correspondent for notices title", i => i.CNTitle);
            csvWriter.DefineColumn(@"Correspondent for notices forename", i => i.CNForename);
            csvWriter.DefineColumn(@"Correspondent for notices surname", i => i.CNSurname);

            csvWriter.DefineColumn(@"Correspondent for notices telephone", i => i.CNTelephone, true);
            csvWriter.DefineColumn(@"Correspondent for notices mobile", i => i.CNMobile, true);
            csvWriter.DefineColumn(@"Correspondent for notices fax", i => i.CNFax, true);
            csvWriter.DefineColumn(@"Correspondent for notices email", i => i.CNEmail);

            //address
            csvWriter.DefineColumn(@"Correspondent for notices address line1", i => i.CNPrimaryName);
            csvWriter.DefineColumn(@"Correspondent for notices address line2", i => i.CNSecondaryName);
            csvWriter.DefineColumn(@"Correspondent for notices street", i => i.CNStreet);
            csvWriter.DefineColumn(@"Correspondent for notices town", i => i.CNTown);
            csvWriter.DefineColumn(@"Correspondent for notices locality", i => i.CNLocality);
            csvWriter.DefineColumn(@"Correspondent for notices administrative area", i => i.CNAdministrativeArea);
            csvWriter.DefineColumn(@"Correspondent for notices post code", i => i.CNPostcode);
            csvWriter.DefineColumn(@"Correspondent for notices country", i => i.CNCountry);

            //company or partnership details based on organisation type
            csvWriter.DefineColumn(@"Reg. Off. or PPoB title", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactTitle : i.PPOBContactTitle);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB forename", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactForename : i.PPOBContactForename);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB surname", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactSurname : i.PPOBContactSurname);

            csvWriter.DefineColumn(@"Reg. Off. or PPoB telephone", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactTelephone : i.PPOBContactTelephone, true);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB mobile", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactMobile : i.PPOBContactMobile, true);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB fax", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactFax : i.PPOBContactFax, true);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB email", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactEmail : i.PPOBContactEmail);

            //address
            csvWriter.DefineColumn(@"Reg. Off. or PPoB address line1", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactPrimaryName : i.PPOBContactPrimaryName);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB address line2", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactSecondaryName : i.PPOBContactSecondaryName);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB street", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactStreet : i.PPOBContactStreet);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB town", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactTown : i.PPOBContactTown);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB locality", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactLocality : i.PPOBContactLocality);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB administrative area", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactAdministrativeArea : i.PPOBContactAdministrativeArea);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB post code", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactPostcode : i.PPOBContactPostcode);
            csvWriter.DefineColumn(@"Reg. Off. or PPoB country", i => !string.IsNullOrEmpty(i.CompanyName) ? i.CompanyContactCountry : i.PPOBContactCountry);

            //overseas producer details
            csvWriter.DefineColumn(@"Overseas producer name", i => i.OverseasProducerName);

            csvWriter.DefineColumn(@"Overseas producer title", i => i.OverseasContactTitle);
            csvWriter.DefineColumn(@"Overseas producer forename", i => i.OverseasContactForename);
            csvWriter.DefineColumn(@"Overseas producer surname", i => i.OverseasContactSurname);

            csvWriter.DefineColumn(@"Overseas producer telephone", i => i.OverseasContactTelephone, true);
            csvWriter.DefineColumn(@"Overseas producer mobile", i => i.OverseasContactMobile, true);
            csvWriter.DefineColumn(@"Overseas producer fax", i => i.OverseasContactFax, true);
            csvWriter.DefineColumn(@"Overseas producer email", i => i.OverseasContactEmail);

            //address
            csvWriter.DefineColumn(@"Overseas producer address line1", i => i.OverseasContactPrimaryName);
            csvWriter.DefineColumn(@"Overseas producer address line2", i => i.OverseasContactSecondaryName);
            csvWriter.DefineColumn(@"Overseas producer street", i => i.OverseasContactStreet);
            csvWriter.DefineColumn(@"Overseas producer town", i => i.OverseasContactTown);
            csvWriter.DefineColumn(@"Overseas producer locality", i => i.OverseasContactLocality);
            csvWriter.DefineColumn(@"Overseas producer administrative area", i => i.OverseasContactAdministrativeArea);
            csvWriter.DefineColumn(@"Overseas producer post code", i => i.OverseasContactPostcode);
            csvWriter.DefineColumn(@"Overseas producer country", i => i.OverseasContactCountry);

            var outOfRangeProducerBrandAmendments = items
                .Where(r => r.BrandNames.Length > MaxBrandNamesLength)
                .Select(r => r.DateRegistered);
            if (outOfRangeProducerBrandAmendments.Any())
            {
                throw new Exception(
                    string.Format("The producer '{0}' has brand names exceeding the maximum allowed length for the following registrations: {1:ddMMyyyy_HHmm}.",
                        request.PRN, string.Join(", ", outOfRangeProducerBrandAmendments)));
            }
            csvWriter.DefineColumn("Brand names", i => i.BrandNames);

            string fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0}_registrationdetailsandchanges_{1:ddMMyyyy_HHmm}.csv",
                request.PRN,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
