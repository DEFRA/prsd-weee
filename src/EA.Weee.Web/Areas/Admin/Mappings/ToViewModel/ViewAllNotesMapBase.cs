namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Extensions;
    using Web.ViewModels.Shared;

    public abstract class ViewAllNotesMapBase<T> where T : IManageEvidenceViewModel, new()
    {
        protected readonly IMapper Mapper;

        protected ViewAllNotesMapBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        protected T CreateModel(ViewAllEvidenceNotesMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var complianceYear =
             source.ManageEvidenceNoteViewModel != null && source.ManageEvidenceNoteViewModel.SelectedComplianceYear > 0
                 ? source.ManageEvidenceNoteViewModel.SelectedComplianceYear
                 : source.CurrentDate.Year;

            var model = new T
            {
                EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(source.NoteData.Results.ToList()),
                ManageEvidenceNoteViewModel = source.ManageEvidenceNoteViewModel
            };

            if (model.ManageEvidenceNoteViewModel != null)
            {
                model.ManageEvidenceNoteViewModel.ComplianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(source.CurrentDate);
                model.ManageEvidenceNoteViewModel.SelectedComplianceYear = complianceYear;
            }

            return model;
        }
    }
}