namespace EA.Weee.RequestHandlers.Admin.AatfReports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Admin.AatfReports;
    using Security;
    using Shared;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class GetAatfAeDetailsCsvHandler : IRequestHandler<GetAatfAeDetailsCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAatfAeDetailsCsvHandler(IWeeeAuthorization authorization,
            WeeeContext context,
            CsvWriterFactory csvWriterFactory,
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<CSVFileData> HandleAsync(GetAatfAeDetailsCsv request)
        {
            authorization.EnsureCanAccessInternalArea();

            if (request.ComplianceYear == 0)
            {
                var message = $"Compliance year cannot be \"{request.ComplianceYear}\".";
                throw new ArgumentException(message);
            }

            var facilityType = request.FacilityType != null ? (int)request.FacilityType : 4;

            List<AatfAeDetailsData> items = await context.StoredProcedures.GetAatfAeDetailsCsvData(
                       request.ComplianceYear, facilityType,
                       request.AuthorityId, request.LocalArea, request.PanArea);

            string type = request.FacilityType.ToString().ToUpper();

            var csvWriter = csvWriterFactory.Create<AatfAeDetailsData>();

            csvWriter.DefineColumn($"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn($"Appropriate authority", i => i.AppropriateAuthorityAbbr);
            if (!request.IsPublicRegister)
            {
                csvWriter.DefineColumn($"WROS Pan Area Team", i => i.PanAreaTeam);
                csvWriter.DefineColumn($"EA Area", i => i.EaArea);
                csvWriter.DefineColumn($"AATF, AE or PCS?", i => i.RecordType);
                csvWriter.DefineColumn($"Name", i => i.Name);
                csvWriter.DefineColumn($"Approval number", i => i.ApprovalNumber);
                csvWriter.DefineColumn($"Status", i => i.Status);
                csvWriter.DefineColumn($"AATF / AE address1", i => i.Address1);
                csvWriter.DefineColumn($"AATF / AE address2", i => i.Address2);
                csvWriter.DefineColumn($"AATF / AE town or city", i => i.TownCity);
                csvWriter.DefineColumn($"AATF / AE county or region", i => i.CountyRegion);
                csvWriter.DefineColumn($"AATF / AE postcode", i => i.PostCode);
                csvWriter.DefineColumn($"AATF / AE country", i => i.Country);
                csvWriter.DefineColumn($"PCS billing reference", i => i.IbisCustomerReference);
                csvWriter.DefineColumn($"PCS obligation type", i => i.ObligationType);
                csvWriter.DefineColumn($"AATF / AE date of approval", i => i.ApprovalDateString);
                csvWriter.DefineColumn($"AATF / AE size", i => i.Size);
                csvWriter.DefineColumn($"Contact first name", i => i.FirstName);
                csvWriter.DefineColumn($"Contact last name", i => i.LastName);
                csvWriter.DefineColumn($"Contact position", i => i.ContactPosition);
                csvWriter.DefineColumn($"Contact address1", i => i.ContactAddress1);
                csvWriter.DefineColumn($"Contact address2", i => i.ContactAddress2);
                csvWriter.DefineColumn($"Contact town or city", i => i.ContactTownCity);
                csvWriter.DefineColumn($"Contact county or region", i => i.ContactCountyRegion);
                csvWriter.DefineColumn($"Contact postcode", i => i.ContactPostcode);
                csvWriter.DefineColumn($"Contact country", i => i.ContactCountry);
                csvWriter.DefineColumn($"Contact phone number", i => i.ContactPhone);
                csvWriter.DefineColumn($"Contact email", i => i.ContactEmail);
                csvWriter.DefineColumn($"Organisation type", i => i.OrganisationTypeString);
                csvWriter.DefineColumn($"Organisation name", i => i.OperatorName);
                csvWriter.DefineColumn($"Organisation business trading name", i => i.OperatorTradingName);
                csvWriter.DefineColumn($"Organisation company registration number", i => i.CompanyRegistrationNumber);
                csvWriter.DefineColumn($"Organisation address1", i => i.OrganisationAddress1);
                csvWriter.DefineColumn($"Organisation address2", i => i.OrganisationAddress2);
                csvWriter.DefineColumn($"Organisation town or city", i => i.OrganisationTownCity);
                csvWriter.DefineColumn($"Organisation county or region", i => i.OrganisationCountyRegion);
                csvWriter.DefineColumn($"Organisation postcode", i => i.OrganisationPostcode);
                csvWriter.DefineColumn($"Organisation country", i => i.OrganisationCountry);
                csvWriter.DefineColumn($"Organisation telephone", i => i.OrganisationTelephone);
                csvWriter.DefineColumn($"Organisation email", i => i.OrganisationEmail);
            }
            else
            {
                csvWriter.DefineColumn($"Name of {type}", i => i.Name);
                csvWriter.DefineColumn($"{type} address", i => i.AatfAddress);
                csvWriter.DefineColumn($"{type} postcode", i => i.PostCode);
                csvWriter.DefineColumn($"{type} country", i => i.Country);
                csvWriter.DefineColumn($"EA Area for the {type}", i => i.EaArea);
                csvWriter.DefineColumn($"{type} approval number", i => i.ApprovalNumber);
                csvWriter.DefineColumn($"Date of approval", i => i.ApprovalDateString);
                csvWriter.DefineColumn($"{type} size", i => i.Size);
                csvWriter.DefineColumn($"{type} status", i => i.Status);
                csvWriter.DefineColumn($"Name of operator", i => i.OperatorName);
                csvWriter.DefineColumn($"Business trading name of operator", i => i.OperatorTradingName);
                csvWriter.DefineColumn($"Operator address ", i => i.OperatorAddress);
                csvWriter.DefineColumn($"Operator postcode", i => i.OrganisationPostcode);
                csvWriter.DefineColumn($"Operator country", i => i.OrganisationCountry);
            }                      

            var fileContent = csvWriter.Write(items);

            var additionalParameters = string.Empty;

            var additionalText = string.Empty;

            var facilityText = string.Empty;

            if (request.FacilityType.HasValue)
            {
                facilityText = $"_{request.FacilityType.ToString().ToUpper()}";
            }

            if (request.AuthorityId.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Abbreviation}";
            }
            if (request.PanArea.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Name}";
            }          

            if (request.IsPublicRegister)
            {
                additionalText = " public register";
            }
            else
            {
                additionalText = "_AATF-AE-PCS-organisation details";
            }

            var fileName =
                $"{request.ComplianceYear}{additionalParameters}{facilityText}{additionalText}_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
