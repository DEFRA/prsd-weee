namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddOffSiteDataAccess
    {
        Task Submit(WeeeReusedSite weeeReusedSite);
    }
}
