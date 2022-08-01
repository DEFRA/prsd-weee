namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.Aatf.ViewModels;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using Core.Shared;
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

        public T MapBase(EvidenceNoteSearchDataResult notes, 
            DateTime currentDate,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            Func<IEnumerable<int>> getComplianceYearList = null)
        {
            Condition.Requires(notes).IsNotNull();

            var complianceYear =
                manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0
                    ? manageEvidenceNoteViewModel.SelectedComplianceYear
                    : currentDate.Year;

            var m = new T
            {
                EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes.Results.ToList()),
                ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel
                {
                    ComplianceYearList = getComplianceYearList != null ? getComplianceYearList() : ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(currentDate),
                    SelectedComplianceYear = complianceYear,
                    ComplianceYearClosed = !WindowHelper.IsDateInComplianceYear(complianceYear, currentDate)
                }
            };

            return m;
        }
    }
}