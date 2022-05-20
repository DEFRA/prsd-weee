namespace EA.Weee.RequestHandlers.AatfReturn.SearchedAatfResultList
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearchedAatfResultListDataAccess
    {
        Task<List<WeeeSearchedAnAatfListData>> GetAnAatfBySearchId(Guid selectedAatfId);
    }
}
