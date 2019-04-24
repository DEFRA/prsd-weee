namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddSelectReportOptionsRequestCreator : IAddSelectReportOptionsRequestCreator
    {
        public AddReturnReportOn ViewModelToRequest(SelectReportOptionsViewModel viewModel)
        {
            var reportOptions = new AddReturnReportOn()
            {
                ReturnId = viewModel.ReturnId,
                SelectedOptions = viewModel.ReportOnQuestions
            };

            return reportOptions;
        }
    }
}