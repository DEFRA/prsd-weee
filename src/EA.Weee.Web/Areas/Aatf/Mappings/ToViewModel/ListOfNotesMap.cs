namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

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