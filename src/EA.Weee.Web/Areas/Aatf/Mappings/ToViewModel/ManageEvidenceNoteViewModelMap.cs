namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.Extensions;
    using EA.Weee.Web.ViewModels.Shared;
    using Helpers;
    using Prsd.Core.Mapper;

    public class ManageEvidenceNoteViewModelMap : IMap<ManageEvidenceNoteTransfer, ManageEvidenceNoteViewModel>
    {
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;

        public ManageEvidenceNoteViewModelMap(IAatfEvidenceHelper aatfEvidenceHelper)
        {
            this.aatfEvidenceHelper = aatfEvidenceHelper;
        }

        public ManageEvidenceNoteViewModel Map(ManageEvidenceNoteTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var singleAatf = source.Aatfs.Where(a =>
                a.FacilityType.Equals(FacilityType.Aatf) && 
                ((int)a.ComplianceYear).Equals(source.ComplianceYear));

            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = source.OrganisationId, 
                AatfId = source.AatfId, 
                AatfName = source.AatfData.Name, 
                SingleAatf = singleAatf.Count().Equals(1),
                ComplianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(source.CurrentDate),
                CanCreateEdit = aatfEvidenceHelper.AatfCanEditCreateNotes(source.Aatfs, source.AatfId, source.ComplianceYear)
            };

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