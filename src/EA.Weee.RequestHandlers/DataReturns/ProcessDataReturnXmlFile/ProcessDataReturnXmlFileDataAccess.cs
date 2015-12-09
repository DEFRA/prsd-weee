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
            try
            {
                var result = await context
                    .DataReturns
                    .Where(dr => dr.Scheme.Id == schemeId && dr.ComplianceYear == complianceYear && dr.Quarter == quarter)
                  .SingleOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }

        public async Task SaveAsync(DataReturnUpload dataUpload)
        {
            context.DataReturnsUploads.Add(dataUpload);
            await context.SaveChangesAsync();
        }

        public async Task SaveAsync(DataReturn dataReturn)
        {
            context.DataReturns.Add(dataReturn);
            await context.SaveChangesAsync();
        }
    }
}
