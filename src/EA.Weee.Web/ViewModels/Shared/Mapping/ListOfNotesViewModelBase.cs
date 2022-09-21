namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.Shared.Paging;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;

    public abstract class ListOfNotesViewModelBase<T> where T : IManageEvidenceViewModel, new()
    {
        protected readonly IMapper Mapper;
        
        protected ListOfNotesViewModelBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public T MapBase(EvidenceNoteSearchDataResult notes,
            int pageNumber,
            int pageSize)
        {
            Condition.Requires(notes).IsNotNull();

            var notesList = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes.Results.ToList());

            var m = new T
            {
                EvidenceNotesDataList = notesList.ToPagedList(pageNumber - 1, pageSize, notes.NoteCount) as PagedList<EvidenceNoteRowViewModel>
            };

            return m;
        }
    }
}