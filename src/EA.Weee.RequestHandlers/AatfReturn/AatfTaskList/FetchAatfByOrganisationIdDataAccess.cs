namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class FetchAatfByOrganisationIdDataAccess : IFetchAatfByOrganisationIdDataAccess
    {
        private readonly WeeeContext context;

        public FetchAatfByOrganisationIdDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Aatf>> FetchAatfByOrganisationId(Guid organisationId)
        {
            return await context.Aatfs.Where(a => a.Operator.Organisation.Id == organisationId).Select(a => a).ToListAsync();
        }
    }
}
