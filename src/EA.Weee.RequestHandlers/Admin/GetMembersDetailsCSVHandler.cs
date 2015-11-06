namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin; 
    using Security;

    internal class GetMembersDetailsCSVHandler : IRequestHandler<GetMemberDetailsCSV, MembersDetailsCSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetMembersDetailsCSVHandler(IWeeeAuthorization authorization, WeeeContext context, CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CC0001:You should use 'var' whenever possible.", Justification = "<Pending>")]
        public async Task<MembersDetailsCSVFileData> HandleAsync(GetMemberDetailsCSV request)
        {
            authorization.EnsureCanAccessInternalArea();

            var items = await context.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(
                       request.ComplianceYear, request.SchemeId, request.CompetentAuthorityId);

            CsvWriter<MembersDetailsCsvData> csvWriter = csvWriterFactory.Create<MembersDetailsCsvData>();
            csvWriter.DefineColumn(@"Scheme name", i => i.SchemeName);
            csvWriter.DefineColumn(@"Scheme Approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn(@"Organisation type", i => i.OrgType);
            csvWriter.DefineColumn(@"Company registration number", i => i.CompanyNumber);
            csvWriter.DefineColumn(@"Partnership names", i => i.Partners);
            csvWriter.DefineColumn(@"Trading name", i => i.TradingName);
            csvWriter.DefineColumn(@"Producer registration number", i => i.PRN);           
            csvWriter.DefineColumn(@"Date & time (GMT) registered", i => i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss"));
            csvWriter.DefineColumn(@"Date & time (GMT) last updated", i => (i.DateRegistered.ToString("dd/MM/yyyy HH:mm:ss").Equals(i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")) ? string.Empty : i.DateAmended.ToString("dd/MM/yyyy HH:mm:ss")));
            csvWriter.DefineColumn(@"Charge band", i => i.ChargeBandType);
            csvWriter.DefineColumn(@"VAT registered", (i => i.VATRegistered ? "Yes" : "No"));
            csvWriter.DefineColumn(@"Annual turnover", i => i.AnnualTurnover);
            csvWriter.DefineColumn(@"Annual turnover band", i => i.AnnualTurnoverBandType);
            csvWriter.DefineColumn(@"EEE placed on market", i => i.EEPlacedOnMarketBandType);
            csvWriter.DefineColumn(@"Obligation Type", i => i.ObligationType);
            csvWriter.DefineColumn(@"SIC codes", i => i.SICCodes);
            csvWriter.DefineColumn(@"Selling technique", i => i.SellingTechniqueType);
            csvWriter.DefineColumn(@"Date ceased to exist", i => i.CeaseToExist.HasValue ? i.CeaseToExist.Value.ToString("dd/MM/yyyy") : string.Empty);

            //correspondences of notices details
            csvWriter.DefineColumn(@"Correspondence contact name", i => string.Format("{0} {1} {2}", i.CNTitle, i.CNForename, i.CNSurname));
            csvWriter.DefineColumn(@"Correspondence telephone", i => i.CNTelephone);
            csvWriter.DefineColumn(@"Correspondence mobile", i => i.CNMobile);
            csvWriter.DefineColumn(@"Correspondence fax", i => i.CNFax);
            csvWriter.DefineColumn(@"Correspondence email", i => i.CNEmail);
            csvWriter.DefineColumn(@"Correspondence address", i => string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", i.CNPrimaryName, i.CNSecondaryName, i.CNStreet, i.CNTown, i.CNLocality, i.CNAdministrativeArea, i.CNPostcode, i.CNCountry));

            //company or partnership details based on organisation type
            csvWriter.DefineColumn(@"Contact name", i => (i.CompanyNumber != string.Empty) ? string.Format("{0} {1} {2}", i.CompanyContactTitle, i.CompanyContactForename, i.CompanyContactSurname) : string.Format("{0} {1} {2}", i.PPOBContactTitle, i.PPOBContactForename, i.PPOBContactSurname));
            csvWriter.DefineColumn(@"Contact telephone", i => (i.CompanyNumber != string.Empty) ? i.CompanyContactTelephone : i.PPOBContactTelephone);
            csvWriter.DefineColumn(@"Contact mobile", i => (i.CompanyNumber != string.Empty) ? i.CompanyContactMobile : i.PPOBContactMobile);
            csvWriter.DefineColumn(@"Contact fax", i => (i.CompanyNumber != string.Empty) ? i.CompanyContactFax : i.PPOBContactFax);
            csvWriter.DefineColumn(@"Contact email", i => (i.CompanyNumber != string.Empty) ? i.CompanyContactEmail : i.PPOBContactEmail);
            csvWriter.DefineColumn(@"Contact address", i => (i.CompanyNumber != string.Empty) ? 
                                                                string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", i.CompanyContactPrimaryName, i.CompanyContactSecondaryName, i.CompanyContactStreet, i.CompanyContactTown, i.CompanyContactLocality, i.CompanyContactAdministrativeArea, i.CompanyContactPostcode, i.CompanyContactCountry)
                                                                :
                                                                string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", i.PPOBContactPrimaryName, i.PPOBContactSecondaryName, i.PPOBContactStreet, i.PPOBContactTown, i.PPOBContactLocality, i.PPOBContactAdministrativeArea, i.PPOBContactPostcode, i.PPOBContactCountry));

            //overseas producer details
            csvWriter.DefineColumn(@"Overseas producer name", i => i.OverseasProducerName);
            csvWriter.DefineColumn(@"Overseas producer contact name", i => string.Format("{0} {1} {2}", i.OverseasContactTitle, i.OverseasContactForename, i.OverseasContactForename));
            csvWriter.DefineColumn(@"Overseas producer telephone", i => i.OverseasContactTelephone);
            csvWriter.DefineColumn(@"Overseas producer mobile", i => i.OverseasContactMobile);
            csvWriter.DefineColumn(@"Overseas producer fax", i => i.OverseasContactFax);
            csvWriter.DefineColumn(@"Overseas producer email", i => i.OverseasContactEmail);
            csvWriter.DefineColumn(@"Overseas producer address", i => string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", i.OverseasContactPrimaryName, i.OverseasContactSecondaryName, i.OverseasContactStreet, i.OverseasContactTown, i.OverseasContactLocality, i.OverseasContactAdministrativeArea, i.OverseasContactPostcode, i.OverseasContactCountry));

            string fileContent = csvWriter.Write(items);

            var fileName = string.Format("{0} - producerdetails_{1:ddMMyyyy_HHmm}.csv",
                request.ComplianceYear,
                DateTime.UtcNow);

            return new MembersDetailsCSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
