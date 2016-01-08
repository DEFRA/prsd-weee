namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;

    public class ProcessDataReturnXmlFileDataAccess : IProcessDataReturnXmlFileDataAccess
    {
        private readonly WeeeContext context;

        public ProcessDataReturnXmlFileDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Schemes.SingleAsync(c => c.OrganisationId == organisationId);
        }

        public async Task AddAndSaveAsync(DataReturnUpload dataUpload)
        {
            context.DataReturnsUploads.Add(dataUpload);
            await context.SaveChangesAsync();
        }
    }
}
