namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnDataToSelectReportOptionsNilViewModelMap : IMap<ReturnData, SelectReportOptionsNilViewModel>
    {
        public SelectReportOptionsNilViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new SelectReportOptionsNilViewModel()
            {
                ReturnId = source.Id,
                OrganisationId = source.OrganisationId,
                QuarterWindowEndDate = source.QuarterWindow.EndDate,
                QuarterWindowStartDate = source.QuarterWindow.StartDate,
                Quarter = source.Quarter.Q.ToString()
            };

            return model;
        }
    }
}