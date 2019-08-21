namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

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
