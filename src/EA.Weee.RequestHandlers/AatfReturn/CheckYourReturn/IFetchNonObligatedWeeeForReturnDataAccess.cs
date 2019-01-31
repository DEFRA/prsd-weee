namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IFetchNonObligatedWeeeForReturnDataAccess
    {
        Task<List<int>> GetDataReturnComplianceYearsForScheme(Guid returnId, bool dcf);
    }
}
