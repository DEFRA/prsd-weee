﻿namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Aatf.Helpers;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Shared;
    using Services;

    public class ManageEvidenceNoteViewModelMap : IMap<ManageEvidenceNoteTransfer, ManageEvidenceNoteViewModel>
    {
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;
        private readonly ConfigurationService configurationService;

        public ManageEvidenceNoteViewModelMap(IAatfEvidenceHelper aatfEvidenceHelper, ConfigurationService configurationService)
        {
            this.aatfEvidenceHelper = aatfEvidenceHelper;
            this.configurationService = configurationService;
        }

        public ManageEvidenceNoteViewModel Map(ManageEvidenceNoteTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var isComplianceYearClosed = !WindowHelper.IsDateInComplianceYear(source.ComplianceYear, source.CurrentDate);            

            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = source.OrganisationId,
                ComplianceYearList = source.ComplianceYearList ?? ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom, source.CurrentDate),
                ComplianceYearClosed = isComplianceYearClosed,
                AatfId = source.AatfId,
                CanDisplayCancelButton = !isComplianceYearClosed
            };

            if (source.Aatfs != null)
            {
                var aatfs = aatfEvidenceHelper.GroupedValidAatfs(source.Aatfs);

                model.SingleAatf = aatfs.Count == 1;
                model.CanCreateEdit = (aatfEvidenceHelper.AatfCanEditCreateNotes(source.Aatfs, source.AatfId, source.ComplianceYear) &&
                                       WindowHelper.IsDateInComplianceYear(source.ComplianceYear, source.CurrentDate));
            }

            if (source.AatfData != null)
            {
                model.AatfName = source.AatfData.Name;
                var aatfStatus = source.AatfData.AatfStatus;

                model.CanDisplayCancelButton = !isComplianceYearClosed && aatfStatus != AatfStatus.Cancelled;
            }

            if (source.FilterViewModel != null)
            {
                model.FilterViewModel = source.FilterViewModel;
            }

            if (source.RecipientWasteStatusFilterViewModel != null)
            {
                model.RecipientWasteStatusFilterViewModel = source.RecipientWasteStatusFilterViewModel;
            }

            if (source.SubmittedDatesFilterViewModel != null)
            {
                model.SubmittedDatesFilterViewModel = source.SubmittedDatesFilterViewModel;
            }

            model.SelectedComplianceYear = source.ComplianceYear;

            return model;
        }
    }
}