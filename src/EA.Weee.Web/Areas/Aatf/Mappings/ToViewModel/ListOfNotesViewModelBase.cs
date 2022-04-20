namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using ViewModels;

    public abstract class ListOfNotesViewModelBase<T> where T : ManageEvidenceNoteOverviewViewModel, new()
    {
        protected readonly IMapper Mapper;

        protected ListOfNotesViewModelBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public T Map(List<EvidenceNoteData> notes)
        {
            Condition.Requires(notes).IsNotNull();

            var m = new T
            {
                ListOfNotes = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes)
            };

            return m;
        }
    }
}