namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Scheme;

    public class DataReturnContentsGeneratorDataAccess : IDataReturnContentsGeneratorDataAccess
    {
        private readonly WeeeContext context;

        public DataReturnContentsGeneratorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Scheme.Scheme> FetchSchemeAsync(Guid organisationID)
        {
            return await context.Schemes.SingleAsync(s => s.OrganisationId == organisationID);
        }
    }
}
