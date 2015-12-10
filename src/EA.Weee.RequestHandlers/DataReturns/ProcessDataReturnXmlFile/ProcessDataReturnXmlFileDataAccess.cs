namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;

    public class ProcessDataReturnXMLFileDataAccess : IProcessDataReturnXMLFileDataAccess
    {
        private readonly WeeeContext context;

        public ProcessDataReturnXMLFileDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Schemes.SingleAsync(c => c.OrganisationId == organisationId);
        }

        public async Task<DataReturn> FetchDataReturnAsync(Guid schemeId, int complianceYear, int quarter)
        {
            var result = await context
                    .DataReturns
                    .Where(dr => dr.Scheme.Id == schemeId && dr.ComplianceYear == complianceYear && dr.Quarter == quarter)
                  .SingleOrDefaultAsync();
            return result;
        }

        public async Task SaveDataReturnsUploadAsync(DataReturnUpload dataUpload)
        {
            context.DataReturnsUploads.Add(dataUpload);
            await context.SaveChangesAsync();
        }

        public async Task SaveSuccessfulReturnsDataAsync(DataReturnUpload dataUpload, DataReturn dataReturn, DataReturnVersion version)
        {
            context.DataReturnsUploads.Add(dataUpload);
            context.DataReturns.Add(dataReturn);
            context.DataReturnVersions.Add(version);
            await context.SaveChangesAsync();
        }
    }
}
