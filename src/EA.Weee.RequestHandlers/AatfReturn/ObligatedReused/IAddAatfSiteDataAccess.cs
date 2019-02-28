namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddAatfSiteDataAccess
    {
        Task Submit(WeeeReusedSite weeeReusedSite);
    }
}
