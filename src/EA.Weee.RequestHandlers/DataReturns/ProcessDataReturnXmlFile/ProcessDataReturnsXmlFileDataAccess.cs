namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Scheme;

    public class ProcessDataReturnsXmlFileDataAccess : IProcessDataReturnXmlFileDataAccess
    {
        private readonly WeeeContext context;

        public ProcessDataReturnsXmlFileDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Scheme> FetchSchemeByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Schemes.SingleAsync(c => c.OrganisationId == organisationId);
        }

        public async Task SaveAsync(DataReturnsUpload dataReturn)
        {
            context.DataReturnsUploads.Add(dataReturn);
            await context.SaveChangesAsync();
        }
    }
}
