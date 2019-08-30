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

            List<AatfAeDetailsData> items = await context.StoredProcedures.GetAatfAeDetailsCsvData(
                       request.ComplianceYear, (int)request.FacilityType,
                       request.AuthorityId, request.LocalArea, request.PanArea);

            string type = request.FacilityType.ToString().ToUpper();

            var csvWriter = csvWriterFactory.Create<AatfAeDetailsData>();

            csvWriter.DefineColumn($"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn($"Appropriate authority", i => i.AppropriateAuthorityAbbr);
            csvWriter.DefineColumn($"WROS Pan Area Team", i => i.PanAreaTeam);
            csvWriter.DefineColumn($"EA Area", i => i.EaArea);
            csvWriter.DefineColumn($"Name of {type}", i => i.Name);
            csvWriter.DefineColumn($"{type} address1", i => i.Address1);
            csvWriter.DefineColumn($"{type} address2", i => i.Address2);
            csvWriter.DefineColumn($"{type} town or city", i => i.TownCity);
            csvWriter.DefineColumn($"{type} county or region", i => i.CountyRegion);
            csvWriter.DefineColumn($"{type} country", i => i.Country);
            csvWriter.DefineColumn($"{type} postcode", i => i.PostCode);
            csvWriter.DefineColumn($"{type} approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn($"Date of approval", i => i.ApprovalDate);
            csvWriter.DefineColumn($"{type} size", i => i.Size);
            csvWriter.DefineColumn($"{type} status", i => i.Status);
            csvWriter.DefineColumn($"Contact name", i => i.ContactName);
            csvWriter.DefineColumn($"Contact position", i => i.ContactPosition);
            csvWriter.DefineColumn($"Contact address1", i => i.ContactAddress1);
            csvWriter.DefineColumn($"Contact address2", i => i.ContactAddress2);
            csvWriter.DefineColumn($"Contact town or city", i => i.ContactTownCity);
            csvWriter.DefineColumn($"Contact county or region", i => i.ContactCountyRegion);
            csvWriter.DefineColumn($"Contact country", i => i.ContactCountry);
            csvWriter.DefineColumn($"Contact postcode", i => i.ContactPostcode);
            csvWriter.DefineColumn($"Contact email", i => i.ContactEmail);
            csvWriter.DefineColumn($"Contact phone number", i => i.ContactPhone);
            csvWriter.DefineColumn($"Organisation name", i => i.OrganisationName);
            csvWriter.DefineColumn($"Organisation address1", i => i.OrganisationAddress1);
            csvWriter.DefineColumn($"Organisation address2", i => i.OrganisationAddress2);
            csvWriter.DefineColumn($"Organisation town or city", i => i.OrganisationTownCity);
            csvWriter.DefineColumn($"Organisation county or region", i => i.OrganisationCountyRegion);
            csvWriter.DefineColumn($"Organisation country", i => i.OrganisationCountry);
            csvWriter.DefineColumn($"Organisation postcode", i => i.OrganisationPostcode);

            var fileContent = csvWriter.Write(items);

            var additionalParameters = string.Empty;

            if (request.AuthorityId.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Abbreviation}";
            }
            if (request.LocalArea.HasValue)
            {
                additionalParameters += $"_{(await commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Name}";
            }

            var fileName =
                $"{request.ComplianceYear}{additionalParameters}_{request.FacilityType.ToString().ToUpper()}_details_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }
    }
}
