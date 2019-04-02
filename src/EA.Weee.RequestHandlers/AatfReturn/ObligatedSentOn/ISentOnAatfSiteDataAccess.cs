namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public interface ISentOnAatfSiteDataAccess
    {
        Task Submit(WeeeSentOn weeeSentOn);

        Task<AatfAddress> GetWeeeSentOnSiteAddress(Guid id);

        Task<AatfAddress> GetWeeeSentOnOperatorAddress(Guid id);

        Task UpdateWithOperatorAddress(WeeeSentOn weeeSentOn, AatfAddress @operator);
    }
}