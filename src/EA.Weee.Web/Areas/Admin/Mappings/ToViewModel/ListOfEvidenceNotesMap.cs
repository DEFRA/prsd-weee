namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.ViewModels.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class ListOfEvidenceNotesMap : IMap<List<AdminEvidenceNoteData>, List<EvidenceNoteRowViewModel>>
    {
        private readonly IMapper mapper;

        public ListOfEvidenceNotesMap(IMapper mapper)
        {
            this.mapper = mapper;
        }
        public List<EvidenceNoteRowViewModel> Map(List<AdminEvidenceNoteData> source)
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
