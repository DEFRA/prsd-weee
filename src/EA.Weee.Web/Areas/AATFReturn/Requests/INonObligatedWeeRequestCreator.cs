namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn;

    public interface INonObligatedWeeRequestCreator : IRequestCreator<NonObligatedValuesViewModel, NonObligatedRequest>
    { 
    }
}