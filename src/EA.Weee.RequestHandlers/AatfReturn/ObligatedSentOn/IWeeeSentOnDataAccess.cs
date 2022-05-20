namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWeeeSentOnDataAccess
    {
        Task Submit(WeeeSentOn weeeSentOn);

        Task Submit(List<WeeeSentOn> weeeSentOn);

        Task<AatfAddress> GetWeeeSentOnSiteAddress(Guid id);

        Task<AatfAddress> GetWeeeSentOnOperatorAddress(Guid id);

        Task UpdateWithOperatorAddress(WeeeSentOn weeeSentOn, AatfAddress address);

        Task<List<WeeeSentOn>> GetWeeeSentOnByReturnAndAatf(Guid aatfId, Guid returnId);

        Task<List<WeeeSentOn>> GetWeeeSentOnByReturn(Guid returnId);

        Task<WeeeSentOn> GetWeeeSentOnById(Guid weeeSentOnId);

        Task<int> GetWeeeSentOnByOperatorId(Guid operatorAddressId);

        Task<int> GetWeeeSentOnBySiteId(Guid siteAddressId);
    }
}