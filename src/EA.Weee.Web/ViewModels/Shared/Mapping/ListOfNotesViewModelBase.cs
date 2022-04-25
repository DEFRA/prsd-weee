namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;

    public abstract class ListOfNotesViewModelBase<T> where T : IEvidenceNoteRowViewModel, new()
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
                EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(notes)
            };

            return m;
        }
    }
}