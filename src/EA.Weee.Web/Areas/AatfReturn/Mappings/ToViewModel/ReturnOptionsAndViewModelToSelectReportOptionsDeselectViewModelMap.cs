namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnOptionsAndViewModelToSelectReportOptionsDeselectViewModelMap : IMap<SelectReportOptionsViewModel, SelectReportOptionsDeselectViewModel>
    {
        public SelectReportOptionsDeselectViewModel Map(SelectReportOptionsViewModel source)
        {
            Guard.ArgumentNotNull(() => source, source);
            
            var viewModel = new SelectReportOptionsDeselectViewModel()
            {
                ReturnId = source.ReturnId,
                OrganisationId = source.OrganisationId,
                ReportOnQuestions = source.ReportOnQuestions,
                DcfSelectedValue = source.DcfSelectedValue
            };

            return viewModel;
        }
    }
}