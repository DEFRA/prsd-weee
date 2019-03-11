namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
    using System.Threading.Tasks;

    public interface IAddSentOnAatfSiteDataAccess
    {
        Task Submit(WeeeSentOn weeeSentOn);
    }
}