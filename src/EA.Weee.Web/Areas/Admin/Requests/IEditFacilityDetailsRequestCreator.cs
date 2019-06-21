namespace EA.Weee.Web.Areas.Admin.Requests
{
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Requests.Base;

    public interface IEditFacilityDetailsRequestCreator : IRequestCreator<FacilityViewModelBase, EditAatfDetails>
    {
    }
}
