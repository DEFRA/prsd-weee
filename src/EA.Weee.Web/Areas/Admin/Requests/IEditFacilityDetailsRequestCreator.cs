namespace EA.Weee.Web.Areas.Admin.Requests
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Requests.Base;
    using Weee.Requests.Admin.Aatf;

    public interface IEditFacilityDetailsRequestCreator : IRequestCreator<FacilityViewModelBase, EditAatfDetails>
    {
    }
}
