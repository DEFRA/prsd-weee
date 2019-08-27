namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using System;

    public class ReturnStatusToReturnDisplayOptionsMap : IMap<(ReturnStatus status, QuarterWindow quarterWindow, DateTime systemDateTime), ReturnsListDisplayOptions>
    {
        public ReturnsListDisplayOptions Map((ReturnStatus status, QuarterWindow quarterWindow, DateTime systemDateTime) source)
        {
            var options = new ReturnsListDisplayOptions();

            switch (source.status)
            {
                case ReturnStatus.Created:
                    options.DisplayContinue = source.quarterWindow.IsOpen(source.systemDateTime);
                    break;
                case ReturnStatus.Submitted:
                    options.DisplayEdit = source.quarterWindow.IsOpen(source.systemDateTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            options.DisplaySummary = source.status == ReturnStatus.Submitted;

            return options;
        }
    }
}