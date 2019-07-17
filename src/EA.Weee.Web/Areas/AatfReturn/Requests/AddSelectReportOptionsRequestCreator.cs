namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using System.Linq;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddSelectReportOptionsRequestCreator : IAddSelectReportOptionsRequestCreator
    {
        public AddReturnReportOn ViewModelToRequest(SelectReportOptionsModelBase viewModel)
        {
            var reportOptions = new AddReturnReportOn()
            {
                ReturnId = viewModel.ReturnId,
                SelectedOptions = viewModel.SelectedOptions,
                DeselectedOptions = viewModel.DeSelectedOptions,
                Options = viewModel.ReportOnQuestions,
                DcfSelectedValue = viewModel.DcfQuestionSelected,
            };

            return reportOptions;
        }
    }
}