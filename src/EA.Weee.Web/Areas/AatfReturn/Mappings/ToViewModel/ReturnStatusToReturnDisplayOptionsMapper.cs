namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core.Mapper;

    public class ReturnStatusToReturnDisplayOptionsMapper : IMap<ReturnStatus, ReturnsListDisplayOptions>
    {
        public ReturnsListDisplayOptions Map(ReturnStatus source)
        {
            var options = new ReturnsListDisplayOptions();

            if (source == ReturnStatus.Created)
            {
                options.DisplayContinue = true;
            }
            else
            {
                options.DisplayEdit = true;
            }

            options.DisplaySummary = true;

            return options;
        }
    }
}