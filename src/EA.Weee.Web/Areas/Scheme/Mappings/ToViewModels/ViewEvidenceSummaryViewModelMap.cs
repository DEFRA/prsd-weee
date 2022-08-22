﻿namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Shared.ToViewModels;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewEvidenceSummaryViewModelMap : ObligationEvidenceSummaryDataToViewModel<SummaryEvidenceViewModel, ViewEvidenceSummaryViewModelMapTransfer>,
        IMap<ViewEvidenceSummaryViewModelMapTransfer, SummaryEvidenceViewModel>
    {
        public SummaryEvidenceViewModel Map(ViewEvidenceSummaryViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var model = new SummaryEvidenceViewModel()
            {
                ObligationEvidenceValues = new List<ObligationEvidenceValue>(),
            };

            var complianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(source.CurrentDate);

            model.ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel
            {
                ComplianceYearList = complianceYearList ?? ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(source.CurrentDate),
                SelectedComplianceYear = source.ComplianceYear,
                ComplianceYearClosed = !WindowHelper.IsDateInComplianceYear(source.ComplianceYear, source.CurrentDate)
            };

            model.SchemeInfo = source.Scheme;
            model.OrganisationId = source.OrganisationId;

            MapToViewModel(model, source);

            return model;
        }
    }
}
