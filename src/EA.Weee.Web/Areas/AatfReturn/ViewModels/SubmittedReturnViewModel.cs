namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using Core.AatfReturn;
    using Core.DataReturns;

    public class SubmittedReturnViewModel : ReturnViewModelBase
    {
        public SubmittedReturnViewModel()
        {
        }

        public SubmittedReturnViewModel(Quarter quarter, QuarterWindow window, int year) : base(quarter, window, year)
        {
        }
    }
}