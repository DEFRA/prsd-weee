namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn.NonObligated;

    public interface INonObligatedWeeeRequestCreator : IRequestCreator<NonObligatedValuesViewModel, NonObligated>
    { 
    }
}