namespace EA.Weee.Web.ViewModels.Returns
{
    using Core.AatfReturn;
    using System;

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