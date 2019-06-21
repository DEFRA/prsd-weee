namespace EA.Weee.Web.ViewModels.Returns
{
    using System;
    using Core.AatfReturn;

    public class SubmittedReturnViewModel : ReturnViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public SubmittedReturnViewModel()
        {
        }

        public SubmittedReturnViewModel(ReturnData returnData) : base(returnData)
        {
        }
    }
}