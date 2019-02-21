namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using ViewModels;
    using Web.Requests.Base;
    using Weee.Requests.AatfReturn.NonObligated;

    public interface INonObligatedWeeRequestCreator : IRequestCreator<NonObligatedValuesViewModel, AddNonObligated>
    { 
    }
}