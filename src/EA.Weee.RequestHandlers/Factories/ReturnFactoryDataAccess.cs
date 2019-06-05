namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;

    public class ReturnFactoryDataAccess : IReturnFactoryDataAccess
    {
        private readonly WeeeContext context;

        public ReturnFactoryDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IList<Aatf>> FetchAatfsByOrganisationFacilityTypeListAndYear(Guid organisationId, int year, EA.Weee.Core.AatfReturn.FacilityType facilityType)
        {
            return await context.Aatfs.Where(a => a.Organisation.Id == organisationId && a.FacilityType.Value == (int)facilityType && a.ComplianceYear == year).ToListAsync();
        }
    }
}
