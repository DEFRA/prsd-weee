namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Shared.Paging;
    using CuttingEdge.Conditions;
    using Extensions;
    using Prsd.Core.Mapper;
    using Services;

    public abstract class ListOfNotesViewModelBase<T> where T : IManageEvidenceViewModel, new()
    {
        protected readonly IMapper Mapper;
        
        protected ListOfNotesViewModelBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public T MapBase(EvidenceNoteSearchDataResult notes, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            int pageNumber,
            int pageSize,
            IEnumerable<int> complianceYearList = null)
        {
            Condition.Requires(notes).IsNotNull();

            var complianceYear =
                manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0
                    ? manageEvidenceNoteViewModel.SelectedComplianceYear
                    : (complianceYearList != null && complianceYearList.Any() ? complianceYearList.ElementAt(0) : currentDate.Year);

            var notesList = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes.Results.ToList());

            var m = new T
            {
                EvidenceNotesDataList = notesList.ToPagedList(pageNumber - 1, pageSize, notes.NoteCount) as PagedList<EvidenceNoteRowViewModel>,
                ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel
                {
                    ComplianceYearList = complianceYearList ?? ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(currentDate),
                    SelectedComplianceYear = complianceYear,
                    ComplianceYearClosed = !WindowHelper.IsDateInComplianceYear(complianceYear, currentDate)
                }
            };

            return m;
        }
    }
}