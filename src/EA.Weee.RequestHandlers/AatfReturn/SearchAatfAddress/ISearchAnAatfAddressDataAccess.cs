namespace EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearchAnAatfAddressDataAccess
    {
        Task<List<ReturnAatfAddressResult>> GetSearchAnAatfAddressBySearchTerm(GetSearchAatfAddress searchAatfAddress);
    }
}
