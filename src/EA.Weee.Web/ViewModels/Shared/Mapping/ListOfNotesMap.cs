namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class ListOfNotesMap : IMap<List<EvidenceNoteData>, List<EvidenceNoteRowViewModel>>
    {
        private readonly IMapper mapper;

        public ListOfNotesMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public List<EvidenceNoteRowViewModel> Map(List<EvidenceNoteData> source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var notes = new List<EvidenceNoteRowViewModel>();

            foreach (var res in source)
            {
                notes.Add(mapper.Map<EvidenceNoteRowViewModel>(res));
            }
            
            return notes;
        }
    }
}