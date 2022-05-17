namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Extensions;
    using Prsd.Core.Mapper;

    public abstract class ListOfNotesViewModelBase<T> where T : IManageEvidenceViewModel, new()
    {
        protected readonly IMapper Mapper;

        protected ListOfNotesViewModelBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public T Map(List<EvidenceNoteData> notes, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel)
        {
            Condition.Requires(notes).IsNotNull();

            var m = new T
            {
                EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes),
                ManageEvidenceNoteViewModel =
                {
                    ComplianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(currentDate),
                    SelectedComplianceYear = manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0 ? manageEvidenceNoteViewModel.SelectedComplianceYear : currentDate.Year
                }
            };

            return m;
        }
    }
}