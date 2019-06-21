namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;

    public interface IFetchAatfByOrganisationIdDataAccess
    {
        Task<List<Aatf>> FetchAatfByOrganisationId(Guid organisationId);
    }
}
