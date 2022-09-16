namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
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

            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = source.OrganisationId,
                ComplianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom, source.CurrentDate),
                CanCreateEdit = (aatfEvidenceHelper.AatfCanEditCreateNotes(source.Aatfs, source.AatfId, source.ComplianceYear) && 
                                 WindowHelper.IsDateInComplianceYear(source.ComplianceYear, source.CurrentDate)),
                ComplianceYearClosed = !WindowHelper.IsDateInComplianceYear(source.ComplianceYear, source.CurrentDate),
                AatfId = source.AatfId
            };

            if (source.AatfData != null)
            {
                var aatfs = aatfEvidenceHelper.GroupedValidAatfs(source.Aatfs);

                model.AatfName = source.AatfData.Name;
                model.SingleAatf = aatfs.Count == 1;
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