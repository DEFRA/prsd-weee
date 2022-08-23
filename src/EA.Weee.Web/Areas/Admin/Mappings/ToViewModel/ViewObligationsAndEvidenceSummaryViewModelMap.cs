namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Web.Areas.Shared.ToViewModels;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;

    public class ViewObligationsAndEvidenceSummaryViewModelMap : ObligationEvidenceSummaryDataToViewModel<ViewObligationsAndEvidenceSummaryViewModel, ViewObligationsAndEvidenceSummaryViewModelMapTransfer>,
        IMap<ViewObligationsAndEvidenceSummaryViewModelMapTransfer, ViewObligationsAndEvidenceSummaryViewModel>
    {
        public ViewObligationsAndEvidenceSummaryViewModelMap(ITonnageUtilities tonnageUtilities) : base(tonnageUtilities)
        {
        }

        public ViewObligationsAndEvidenceSummaryViewModel Map(ViewObligationsAndEvidenceSummaryViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var schemeList = new List<Core.Scheme.OrganisationSchemeData>();
            if (source.SchemeData != null)
            {
                schemeList = source.SchemeData.Select(s => new Core.Scheme.OrganisationSchemeData()
                {
                    DisplayName = s.SchemeName,
                    Id = s.Id
                }).GroupBy(s => s.Id)
                    .Select(s => s.First())
                    .OrderBy(s => s.DisplayName)
                    .ToList();
            }

            var model = new ViewObligationsAndEvidenceSummaryViewModel
            {
                ObligationEvidenceValues = new List<ObligationEvidenceValue>(),
                DisplayNoDataMessage = !source.ComplianceYears.Any(),
                ComplianceYearList = source.ComplianceYears,
                SchemeList = schemeList,
                SchemeId = source.SchemeData != null ? schemeList.Any(s => s.Id == source.SchemeId) ? source.SchemeId : null : source.SchemeId
            };

            MapToViewModel(model, source);

            return model;
        }
    }
}