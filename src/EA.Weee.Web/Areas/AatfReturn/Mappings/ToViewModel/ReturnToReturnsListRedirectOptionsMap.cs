namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;

    public class ReturnToReturnsListRedirectOptionsMap : IMap<ReturnData, ReturnsListRedirectOptions>
    {
        public ReturnsListRedirectOptions Map(ReturnData source)
        {
            var model = new ReturnsListRedirectOptions { RedirectReportingOptions = !source.ReturnReportOns.Any() };

            if (!model.RedirectReportingOptions)
            {
                model.RedirectSelectYourPcs = source.ReturnReportOns.Any(s => s.ReportOnQuestionId == (int)ReportOnQuestionEnum.WeeeReceived)
                    && !source.SchemeDataItems.Any();
            }

            if (!model.RedirectReportingOptions && !model.RedirectSelectYourPcs)
            {
                model.RedirectTaskList = true;
            }

            return model;
        }
    }
}