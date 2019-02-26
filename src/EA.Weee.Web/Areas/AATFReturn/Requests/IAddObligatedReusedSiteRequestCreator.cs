namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public interface IAddObligatedReusedSiteRequestCreator : IRequestCreator<ReusedOffSiteCreateViewModel, AddOffSite>
    {
    }
}