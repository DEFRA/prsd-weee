namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using Prsd.Core.Mapper;

    public class ReturnStatusToReturnDisplayOptionsMap : IMap<(ReturnStatus status, QuarterWindow quarterWindow), ReturnsListDisplayOptions>
    {
        public ReturnsListDisplayOptions Map((ReturnStatus status, QuarterWindow quarterWindow) source)
        {
            var options = new ReturnsListDisplayOptions();

            if (source.status == ReturnStatus.Created)
            {
                options.DisplayContinue = QuarterHelper.IsOpenForReporting(source.quarterWindow);
            }
            else
            {
                options.DisplayEdit = true;
            }

            options.DisplaySummary = source.status == ReturnStatus.Submitted;

            return options;
        }
    }
}