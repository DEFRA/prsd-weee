namespace EA.Weee.Web.Requests
{
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.ViewModels.Shared.Aatf;

    public interface IEditAatfContactRequestCreator : IRequestCreator<AatfEditContactAddressViewModel, EditAatfContact>
    {
    }
}
