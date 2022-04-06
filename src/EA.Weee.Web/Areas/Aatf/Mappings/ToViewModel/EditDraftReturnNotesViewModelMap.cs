namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class EditDraftReturnNotesViewModelMap : IMap<EditDraftReturnNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        private readonly IMapper mapper;

        public EditDraftReturnNotesViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditDraftReturnedNotesViewModel Map(EditDraftReturnNotesViewModelTransfer source)
        {
            var model = new EditDraftReturnedNotesViewModel();

            if (source != null && source.Notes.Any())
            {
                foreach (var res in source.Notes)
                {
                    model.ListOfNotes.Add(mapper.Map<EditDraftReturnedNote>(new EditDraftReturnedNotesModel(res.Reference, res.SchemeData.SchemeName, res.Status, res.WasteType, res.Id, res.Type)));
                }
            }

            return model;
        }
    }
}