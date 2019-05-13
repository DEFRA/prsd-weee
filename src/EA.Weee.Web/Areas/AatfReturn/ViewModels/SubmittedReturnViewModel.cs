namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using Core.AatfReturn;
    using Core.DataReturns;

    public class SubmittedReturnViewModel : ReturnViewModelBase
    {
        public Guid OrgansationId { get; set; }

        public SubmittedReturnViewModel()
        {
        }

        public SubmittedReturnViewModel(ReturnData returnData) : base(returnData)
        {
        }
    }
}