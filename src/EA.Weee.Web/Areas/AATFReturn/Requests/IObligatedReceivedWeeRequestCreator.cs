namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public interface IObligatedReceivedWeeRequestCreator : IRequestCreator<ObligatedReceivedViewModel, AddObligatedReceived>
    {
    }
}
