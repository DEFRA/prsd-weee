namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Helpers;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class EditNoteViewModelMap : IMap<EditNoteMapTransfer, EvidenceNoteViewModel>
    {
        public EvidenceNoteViewModel Map(EditNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new EvidenceNoteViewModel
            {
                Status = source.NoteData.Status,
                Reference = source.NoteData.Reference,
                Type = source.NoteData.Type,
                OrganisationId = source.OrganisationId,
                AatfId = source.AatfId,
                SchemeList = source.Schemes,
                ProtocolList = new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"),
                WasteTypeList = new SelectList(EnumHelper.GetValues(typeof(WasteType)), "Key", "Value"),
                ReceivedId = source.NoteData.RecipientId,
                StartDate = source.NoteData.StartDate,
                EndDate = source.NoteData.EndDate,
                WasteTypeValue = source.NoteData.WasteType,
                ProtocolValue = source.NoteData.Protocol
            };

            if (source.ExistingModel != null)
            {
                model.CategoryValues = source.ExistingModel.CategoryValues;
            }
            else
            {
                foreach (var evidenceTonnageData in source.NoteData.EvidenceTonnageData)
                {
                    var category = model.CategoryValues.FirstOrDefault(c => c.CategoryId.Equals(evidenceTonnageData.CategoryId.ToInt()));

                    if (category != null)
                    {
                        category.Received = evidenceTonnageData.Received.ToTonnageEditDisplay();
                        category.Reused = evidenceTonnageData.Reused.ToTonnageEditDisplay();
                        category.Id = evidenceTonnageData.Id;
                    }
                }
            }
            return model;
        }
    }
}